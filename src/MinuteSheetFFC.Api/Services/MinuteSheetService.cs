using Microsoft.EntityFrameworkCore;
using MinuteSheetFFC.Application.DTOs.Common;
using MinuteSheetFFC.Application.DTOs.MinuteSheet;
using MinuteSheetFFC.Application.DTOs.Workflow;
using MinuteSheetFFC.Application.Interfaces;
using MinuteSheetFFC.Domain.Entities;
using MinuteSheetFFC.Domain.Enums;
using MinuteSheetFFC.Infrastructure.Data;
using MinuteSheetFFC.WorkflowEngine.Services;

namespace MinuteSheetFFC.Api.Services;

public class MinuteSheetService : IMinuteSheetService
{
    private readonly AppDbContext _db;
    private readonly RouteGenerator _routeGenerator;
    private readonly StageManager _stageManager;

    public MinuteSheetService(AppDbContext db, RouteGenerator routeGenerator, StageManager stageManager)
    {
        _db = db;
        _routeGenerator = routeGenerator;
        _stageManager = stageManager;
    }

    public async Task<PagedResponse<MinuteSheetListDto>> GetMinuteSheetsAsync(MinuteSheetFilterDto filter)
    {
        var query = _db.MinuteSheetRequests.Include(m => m.Requester).Include(m => m.CurrentActioner).Include(m => m.RequestType).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var s = filter.Search.ToLower();
            query = query.Where(m => m.Subject.ToLower().Contains(s) || m.ReferenceNumber.ToLower().Contains(s));
        }
        if (!string.IsNullOrWhiteSpace(filter.Status) && Enum.TryParse<RequestStatus>(filter.Status, true, out var st)) query = query.Where(m => m.Status == st);
        if (!string.IsNullOrWhiteSpace(filter.RequesterPNo)) query = query.Where(m => m.RequesterPNo == filter.RequesterPNo);
        if (!string.IsNullOrWhiteSpace(filter.CurrentActionerPNo)) query = query.Where(m => m.CurrentActionerPNo == filter.CurrentActionerPNo);
        if (!string.IsNullOrWhiteSpace(filter.Priority) && Enum.TryParse<Priority>(filter.Priority, true, out var pr)) query = query.Where(m => m.Priority == pr);
        if (filter.DateFrom.HasValue) query = query.Where(m => m.CreatedAt >= filter.DateFrom.Value);
        if (filter.DateTo.HasValue) query = query.Where(m => m.CreatedAt <= filter.DateTo.Value);
        if (filter.BudgetMin.HasValue) query = query.Where(m => m.EstimatedBudget >= filter.BudgetMin.Value);
        if (filter.BudgetMax.HasValue) query = query.Where(m => m.EstimatedBudget <= filter.BudgetMax.Value);

        var total = await query.CountAsync();
        query = filter.SortDir?.ToLower() == "asc" ? query.OrderBy(m => m.CreatedAt) : query.OrderByDescending(m => m.CreatedAt);

        var items = await query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).Select(m => new MinuteSheetListDto
        {
            Id = m.Id, ReferenceNumber = m.ReferenceNumber, Subject = m.Subject,
            RequestTypeCode = m.RequestType.Code, RequestTypeName = m.RequestType.Name,
            EstimatedBudget = m.EstimatedBudget, Priority = m.Priority.ToString(),
            IsConfidential = m.IsConfidential, WorkflowMode = m.WorkflowMode.ToString(),
            Status = m.Status.ToString(), RequesterPNo = m.RequesterPNo,
            RequesterName = m.Requester.Name, CurrentActionerPNo = m.CurrentActionerPNo,
            CurrentActionerName = m.CurrentActioner != null ? m.CurrentActioner.Name : null,
            CreatedAt = m.CreatedAt, SubmittedAt = m.SubmittedAt
        }).ToListAsync();

        return new PagedResponse<MinuteSheetListDto> { Data = items, Page = filter.Page, PageSize = filter.PageSize, TotalCount = total };
    }

    public async Task<ApiResponse<MinuteSheetDetailDto>> GetMinuteSheetAsync(int id)
    {
        var m = await _db.MinuteSheetRequests
            .Include(x => x.Requester).Include(x => x.CurrentActioner).Include(x => x.RequestType)
            .Include(x => x.Stages.OrderBy(s => s.StageOrder))
            .Include(x => x.History.OrderByDescending(h => h.Timestamp))
            .Include(x => x.Attachments)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (m == null) return ApiResponse<MinuteSheetDetailDto>.Fail("Request not found.");

        return ApiResponse<MinuteSheetDetailDto>.Ok(new MinuteSheetDetailDto
        {
            Id = m.Id, ReferenceNumber = m.ReferenceNumber, Subject = m.Subject, Body = m.Body,
            RequestTypeCode = m.RequestType.Code, RequestTypeName = m.RequestType.Name,
            EstimatedBudget = m.EstimatedBudget, Priority = m.Priority.ToString(),
            IsConfidential = m.IsConfidential, WorkflowMode = m.WorkflowMode.ToString(),
            Status = m.Status.ToString(), RequesterPNo = m.RequesterPNo, RequesterName = m.Requester.Name,
            CurrentActionerPNo = m.CurrentActionerPNo, CurrentActionerName = m.CurrentActioner?.Name,
            CurrentStageOrder = m.CurrentStageOrder, CreatedAt = m.CreatedAt,
            SubmittedAt = m.SubmittedAt, CompletedAt = m.CompletedAt,
            Stages = m.Stages.Select(s => new WorkflowStageDto
            {
                Id = s.Id, StageOrder = s.StageOrder, ActionerPNo = s.ActionerPNo,
                ActionerName = s.ActionerName, ActionerDesignation = s.ActionerDesignation,
                ActionType = s.ActionType.ToString(), Status = s.Status.ToString(),
                Action = s.Action?.ToString(), Remarks = s.Remarks,
                ActionedAt = s.ActionedAt, Source = s.Source.ToString()
            }).ToList(),
            History = m.History.Select(h => new WorkflowHistoryDto
            {
                Id = h.Id, ActionerPNo = h.ActionerPNo, ActionerName = h.ActionerName,
                Action = h.Action.ToString(), PreviousStatus = h.PreviousStatus.ToString(),
                NewStatus = h.NewStatus.ToString(), Remarks = h.Remarks,
                StageOrder = h.StageOrder, Timestamp = h.Timestamp
            }).ToList(),
            Attachments = m.Attachments.Select(a => new MinuteSheetFFC.Application.DTOs.MinuteSheet.AttachmentDto
            {
                Id = a.Id, FileName = a.FileName, FileType = a.FileType,
                FileSize = a.FileSize, UploadedByPNo = a.UploadedByPNo, UploadedAt = a.UploadedAt
            }).ToList()
        });
    }

    public async Task<ApiResponse<MinuteSheetDetailDto>> CreateAsync(CreateMinuteSheetDto dto)
    {
        var emp = await _db.Employees.FindAsync(dto.RequesterPNo);
        if (emp == null) return ApiResponse<MinuteSheetDetailDto>.Fail("Requester not found.");

        var count = await _db.MinuteSheetRequests.CountAsync() + 1;
        var refNum = $"MS-{DateTime.UtcNow.Year}-{count:D5}";

        var request = new MinuteSheetRequest
        {
            ReferenceNumber = refNum,
            Subject = dto.Subject,
            Body = dto.Body,
            RequestTypeId = dto.RequestTypeId,
            EstimatedBudget = dto.EstimatedBudget,
            Priority = Enum.TryParse<Priority>(dto.Priority, true, out var p) ? p : Priority.Normal,
            IsConfidential = dto.IsConfidential,
            WorkflowMode = Enum.TryParse<WorkflowMode>(dto.WorkflowMode, true, out var wm) ? wm : WorkflowMode.Dynamic,
            RequesterPNo = dto.RequesterPNo,
            Status = RequestStatus.Draft
        };

        _db.MinuteSheetRequests.Add(request);
        await _db.SaveChangesAsync();

        // Log creation
        _db.WorkflowHistory.Add(new WorkflowHistory
        {
            MinuteSheetId = request.Id, ActionerPNo = dto.RequesterPNo, ActionerName = emp.Name,
            Action = WorkflowActionType.Create, PreviousStatus = RequestStatus.Draft,
            NewStatus = RequestStatus.Draft, Remarks = "Request created."
        });
        await _db.SaveChangesAsync();

        return await GetMinuteSheetAsync(request.Id);
    }

    public async Task<ApiResponse<MinuteSheetDetailDto>> UpdateAsync(int id, UpdateMinuteSheetDto dto)
    {
        var request = await _db.MinuteSheetRequests.FindAsync(id);
        if (request == null) return ApiResponse<MinuteSheetDetailDto>.Fail("Request not found.");
        if (request.Status != RequestStatus.Draft && request.Status != RequestStatus.Returned)
            return ApiResponse<MinuteSheetDetailDto>.Fail("Can only edit Draft or Returned requests.");

        if (dto.Subject != null) request.Subject = dto.Subject;
        if (dto.Body != null) request.Body = dto.Body;
        if (dto.RequestTypeId.HasValue) request.RequestTypeId = dto.RequestTypeId.Value;
        if (dto.EstimatedBudget.HasValue) request.EstimatedBudget = dto.EstimatedBudget;
        if (dto.Priority != null && Enum.TryParse<Priority>(dto.Priority, true, out var p)) request.Priority = p;
        if (dto.IsConfidential.HasValue) request.IsConfidential = dto.IsConfidential.Value;
        if (dto.WorkflowMode != null && Enum.TryParse<WorkflowMode>(dto.WorkflowMode, true, out var wm)) request.WorkflowMode = wm;

        request.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return await GetMinuteSheetAsync(id);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var request = await _db.MinuteSheetRequests.FindAsync(id);
        if (request == null) return ApiResponse<bool>.Fail("Not found.");
        if (request.Status != RequestStatus.Draft) return ApiResponse<bool>.Fail("Can only delete drafts.");
        _db.MinuteSheetRequests.Remove(request);
        await _db.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "Deleted.");
    }

    public async Task<ApiResponse<MinuteSheetDetailDto>> SubmitAsync(int id, string requesterPNo)
    {
        var request = await _db.MinuteSheetRequests.FindAsync(id);
        if (request == null) return ApiResponse<MinuteSheetDetailDto>.Fail("Not found.");
        if (request.Status != RequestStatus.Draft) return ApiResponse<MinuteSheetDetailDto>.Fail("Can only submit drafts.");
        if (request.RequesterPNo != requesterPNo) return ApiResponse<MinuteSheetDetailDto>.Fail("Only requester can submit.");

        // Find matching workflow rule
        var rule = await _db.WorkflowRules
            .Where(r => r.RequestTypeId == request.RequestTypeId && r.IsActive &&
                   r.BudgetFrom <= (request.EstimatedBudget ?? 0) &&
                   (r.BudgetTo == null || r.BudgetTo >= (request.EstimatedBudget ?? 0)))
            .FirstOrDefaultAsync();

        if (rule == null)
        {
            rule = await _db.WorkflowRules.Where(r => r.RequestTypeId == request.RequestTypeId && r.IsActive).FirstOrDefaultAsync();
        }
        if (rule == null) return ApiResponse<MinuteSheetDetailDto>.Fail("No workflow rule found for this request type.");

        // Generate route and stages
        var preview = await _routeGenerator.GenerateDynamicRouteAsync(request, rule);
        if (!preview.IsValid || preview.Steps.Count == 0)
            return ApiResponse<MinuteSheetDetailDto>.Fail("Cannot generate valid route. " + string.Join("; ", preview.Warnings.Select(w => w.Message)));

        var stages = await _stageManager.CreateStagesFromPreviewAsync(request.Id, preview);

        request.Status = RequestStatus.InReview;
        request.SubmittedAt = DateTime.UtcNow;
        request.CurrentActionerPNo = stages.First().ActionerPNo;
        request.CurrentStageOrder = 1;
        request.UpdatedAt = DateTime.UtcNow;

        var emp = await _db.Employees.FindAsync(requesterPNo);
        _db.WorkflowHistory.Add(new WorkflowHistory
        {
            MinuteSheetId = request.Id, ActionerPNo = requesterPNo, ActionerName = emp?.Name ?? requesterPNo,
            Action = WorkflowActionType.Submit, PreviousStatus = RequestStatus.Draft,
            NewStatus = RequestStatus.InReview, Remarks = $"Request submitted. Route generated with {stages.Count} stages."
        });

        await _db.SaveChangesAsync();
        return await GetMinuteSheetAsync(id);
    }

    public async Task<ApiResponse<bool>> CancelAsync(int id, string requesterPNo)
    {
        var request = await _db.MinuteSheetRequests.FindAsync(id);
        if (request == null) return ApiResponse<bool>.Fail("Not found.");
        if (request.RequesterPNo != requesterPNo) return ApiResponse<bool>.Fail("Only requester can cancel.");
        var allowedStatuses = new[] { RequestStatus.Draft, RequestStatus.InReview, RequestStatus.Returned };
        if (!allowedStatuses.Contains(request.Status)) return ApiResponse<bool>.Fail("Cannot cancel in current status.");

        var prevStatus = request.Status;
        var stages = await _db.WorkflowStages.Where(s => s.MinuteSheetId == id && (s.Status == StageStatus.Pending || s.Status == StageStatus.Active)).ToListAsync();
        foreach (var s in stages) s.Status = StageStatus.Skipped;

        request.Status = RequestStatus.Cancelled;
        request.CurrentActionerPNo = null;
        request.CurrentStageOrder = null;
        request.CompletedAt = DateTime.UtcNow;

        var emp = await _db.Employees.FindAsync(requesterPNo);
        _db.WorkflowHistory.Add(new WorkflowHistory
        {
            MinuteSheetId = id, ActionerPNo = requesterPNo, ActionerName = emp?.Name ?? requesterPNo,
            Action = WorkflowActionType.Cancel, PreviousStatus = prevStatus,
            NewStatus = RequestStatus.Cancelled, Remarks = "Request cancelled by requester."
        });

        await _db.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "Cancelled.");
    }

    public async Task<ApiResponse<MinuteSheetDetailDto>> ResubmitAsync(int id, string requesterPNo)
    {
        var request = await _db.MinuteSheetRequests.FindAsync(id);
        if (request == null) return ApiResponse<MinuteSheetDetailDto>.Fail("Not found.");
        if (request.Status != RequestStatus.Returned) return ApiResponse<MinuteSheetDetailDto>.Fail("Can only resubmit returned requests.");
        if (request.RequesterPNo != requesterPNo) return ApiResponse<MinuteSheetDetailDto>.Fail("Only requester can resubmit.");

        // Reset stages
        var oldStages = await _db.WorkflowStages.Where(s => s.MinuteSheetId == id).ToListAsync();
        _db.WorkflowStages.RemoveRange(oldStages);
        await _db.SaveChangesAsync();

        // Re-generate
        var rule = await _db.WorkflowRules.Where(r => r.RequestTypeId == request.RequestTypeId && r.IsActive && r.BudgetFrom <= (request.EstimatedBudget ?? 0) && (r.BudgetTo == null || r.BudgetTo >= (request.EstimatedBudget ?? 0))).FirstOrDefaultAsync();
        if (rule == null) rule = await _db.WorkflowRules.Where(r => r.RequestTypeId == request.RequestTypeId && r.IsActive).FirstOrDefaultAsync();
        if (rule == null) return ApiResponse<MinuteSheetDetailDto>.Fail("No workflow rule found.");

        var preview = await _routeGenerator.GenerateDynamicRouteAsync(request, rule);
        var stages = await _stageManager.CreateStagesFromPreviewAsync(request.Id, preview);

        request.Status = RequestStatus.InReview;
        request.CurrentActionerPNo = stages.First().ActionerPNo;
        request.CurrentStageOrder = 1;
        request.UpdatedAt = DateTime.UtcNow;

        var emp = await _db.Employees.FindAsync(requesterPNo);
        _db.WorkflowHistory.Add(new WorkflowHistory
        {
            MinuteSheetId = id, ActionerPNo = requesterPNo, ActionerName = emp?.Name ?? requesterPNo,
            Action = WorkflowActionType.Resubmit, PreviousStatus = RequestStatus.Returned,
            NewStatus = RequestStatus.InReview, Remarks = "Request resubmitted with regenerated route."
        });

        await _db.SaveChangesAsync();
        return await GetMinuteSheetAsync(id);
    }
}
