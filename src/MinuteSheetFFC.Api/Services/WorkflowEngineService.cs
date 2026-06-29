using Microsoft.EntityFrameworkCore;
using MinuteSheetFFC.Application.DTOs.Common;
using MinuteSheetFFC.Application.DTOs.Workflow;
using MinuteSheetFFC.Application.Interfaces;
using MinuteSheetFFC.Domain.Enums;
using MinuteSheetFFC.Infrastructure.Data;
using MinuteSheetFFC.WorkflowEngine.Services;

namespace MinuteSheetFFC.Api.Services;

public class WorkflowEngineApiService : IWorkflowEngineService
{
    private readonly AppDbContext _db;
    private readonly RouteGenerator _routeGenerator;
    private readonly StageManager _stageManager;

    public WorkflowEngineApiService(AppDbContext db, RouteGenerator routeGenerator, StageManager stageManager)
    {
        _db = db;
        _routeGenerator = routeGenerator;
        _stageManager = stageManager;
    }

    public async Task<ApiResponse<RoutePreviewDto>> GenerateRoutePreviewAsync(int minuteSheetId)
    {
        var request = await _db.MinuteSheetRequests.FindAsync(minuteSheetId);
        if (request == null) return ApiResponse<RoutePreviewDto>.Fail("Request not found.");

        var rule = await _db.WorkflowRules.Where(r => r.RequestTypeId == request.RequestTypeId && r.IsActive && r.BudgetFrom <= (request.EstimatedBudget ?? 0) && (r.BudgetTo == null || r.BudgetTo >= (request.EstimatedBudget ?? 0))).FirstOrDefaultAsync();
        if (rule == null) rule = await _db.WorkflowRules.Where(r => r.RequestTypeId == request.RequestTypeId && r.IsActive).FirstOrDefaultAsync();
        if (rule == null) return ApiResponse<RoutePreviewDto>.Fail("No workflow rule found.");

        var preview = await _routeGenerator.GenerateDynamicRouteAsync(request, rule);
        return ApiResponse<RoutePreviewDto>.Ok(preview);
    }

    public async Task<ApiResponse<WorkflowActionResultDto>> ProcessActionAsync(int minuteSheetId, WorkflowActionDto action)
    {
        var request = await _db.MinuteSheetRequests.FindAsync(minuteSheetId);
        if (request == null) return ApiResponse<WorkflowActionResultDto>.Fail("Request not found.");

        if (!Enum.TryParse<StageAction>(action.ActionType, true, out var stageAction))
            return ApiResponse<WorkflowActionResultDto>.Fail("Invalid action type.");

        var result = await _stageManager.ProcessActionAsync(request, action.ActionerPNo, stageAction, action.Remarks);
        await _db.SaveChangesAsync();
        return result.Success ? ApiResponse<WorkflowActionResultDto>.Ok(result) : ApiResponse<WorkflowActionResultDto>.Fail(result.Message);
    }

    public async Task<ApiResponse<List<PendingActionDto>>> GetPendingActionsAsync(string actionerPNo)
    {
        var pending = await _db.WorkflowStages
            .Where(s => s.ActionerPNo == actionerPNo && s.Status == StageStatus.Active)
            .Include(s => s.MinuteSheet).ThenInclude(m => m.Requester)
            .Select(s => new PendingActionDto
            {
                MinuteSheetId = s.MinuteSheetId,
                ReferenceNumber = s.MinuteSheet.ReferenceNumber,
                Subject = s.MinuteSheet.Subject,
                RequesterPNo = s.MinuteSheet.RequesterPNo,
                RequesterName = s.MinuteSheet.Requester.Name,
                ActionType = s.ActionType.ToString(),
                StageOrder = s.StageOrder,
                SubmittedAt = s.MinuteSheet.SubmittedAt
            }).ToListAsync();

        return ApiResponse<List<PendingActionDto>>.Ok(pending);
    }
}
