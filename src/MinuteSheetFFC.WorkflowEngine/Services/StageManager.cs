using Microsoft.EntityFrameworkCore;
using MinuteSheetFFC.Application.DTOs.Workflow;
using MinuteSheetFFC.Domain.Entities;
using MinuteSheetFFC.Domain.Enums;
using MinuteSheetFFC.Infrastructure.Data;

namespace MinuteSheetFFC.WorkflowEngine.Services;

public class StageManager
{
    private readonly AppDbContext _db;

    public StageManager(AppDbContext db) => _db = db;

    public async Task<List<WorkflowStage>> CreateStagesFromPreviewAsync(int minuteSheetId, RoutePreviewDto preview)
    {
        var stages = new List<WorkflowStage>();
        foreach (var step in preview.Steps)
        {
            var stage = new WorkflowStage
            {
                MinuteSheetId = minuteSheetId,
                StageOrder = step.StageOrder,
                ActionerPNo = step.ActionerPNo,
                ActionerName = step.ActionerName,
                ActionerDesignation = step.ActionerDesignation,
                ActionType = Enum.TryParse<StageActionType>(step.ActionType, true, out var at) ? at : StageActionType.Review,
                Status = step.StageOrder == 1 ? StageStatus.Active : StageStatus.Pending,
                Source = Enum.TryParse<StageSource>(step.Source, true, out var src) ? src : StageSource.Auto
            };
            _db.WorkflowStages.Add(stage);
            stages.Add(stage);
        }
        await _db.SaveChangesAsync();
        return stages;
    }

    public async Task<WorkflowActionResultDto> ProcessActionAsync(MinuteSheetRequest request, string actionerPNo, StageAction action, string? remarks)
    {
        var currentStage = await _db.WorkflowStages
            .FirstOrDefaultAsync(s => s.MinuteSheetId == request.Id && s.ActionerPNo == actionerPNo && s.Status == StageStatus.Active);

        if (currentStage == null)
            return new WorkflowActionResultDto { Success = false, Message = "No active stage found for this actioner." };

        var previousStatus = request.Status;

        currentStage.Action = action;
        currentStage.Remarks = remarks;
        currentStage.ActionedAt = DateTime.UtcNow;
        currentStage.Status = StageStatus.Completed;

        var emp = await _db.Employees.FindAsync(actionerPNo);

        switch (action)
        {
            case StageAction.Approve:
                var nextStage = await _db.WorkflowStages
                    .Where(s => s.MinuteSheetId == request.Id && s.StageOrder > currentStage.StageOrder && s.Status == StageStatus.Pending)
                    .OrderBy(s => s.StageOrder)
                    .FirstOrDefaultAsync();

                if (nextStage != null)
                {
                    nextStage.Status = StageStatus.Active;
                    request.CurrentActionerPNo = nextStage.ActionerPNo;
                    request.CurrentStageOrder = nextStage.StageOrder;
                }
                else
                {
                    request.Status = RequestStatus.Approved;
                    request.CurrentActionerPNo = null;
                    request.CurrentStageOrder = null;
                    request.CompletedAt = DateTime.UtcNow;
                }
                break;

            case StageAction.Reject:
                request.Status = RequestStatus.Rejected;
                request.CurrentActionerPNo = null;
                request.CurrentStageOrder = null;
                request.CompletedAt = DateTime.UtcNow;
                var pendingStages = await _db.WorkflowStages
                    .Where(s => s.MinuteSheetId == request.Id && s.Status == StageStatus.Pending)
                    .ToListAsync();
                foreach (var s in pendingStages) s.Status = StageStatus.Skipped;
                break;

            case StageAction.Return:
                request.Status = RequestStatus.Returned;
                request.CurrentActionerPNo = null;
                request.CurrentStageOrder = null;
                var remainingStages = await _db.WorkflowStages
                    .Where(s => s.MinuteSheetId == request.Id && s.Status == StageStatus.Pending)
                    .ToListAsync();
                foreach (var s in remainingStages) s.Status = StageStatus.Skipped;
                break;

            case StageAction.Forward:
                break;
        }

        request.UpdatedAt = DateTime.UtcNow;

        var actionType = action switch
        {
            StageAction.Approve => WorkflowActionType.Approve,
            StageAction.Reject => WorkflowActionType.Reject,
            StageAction.Return => WorkflowActionType.Return,
            StageAction.Forward => WorkflowActionType.Forward,
            _ => WorkflowActionType.Approve
        };

        _db.WorkflowHistory.Add(new WorkflowHistory
        {
            MinuteSheetId = request.Id,
            ActionerPNo = actionerPNo,
            ActionerName = emp?.Name,
            Action = actionType,
            PreviousStatus = previousStatus,
            NewStatus = request.Status,
            Remarks = remarks,
            StageOrder = currentStage.StageOrder
        });

        return new WorkflowActionResultDto
        {
            Success = true,
            Message = $"Action '{action}' processed successfully.",
            NewStatus = request.Status.ToString(),
            NextActionerPNo = request.CurrentActionerPNo
        };
    }
}
