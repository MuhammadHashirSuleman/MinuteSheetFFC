using Microsoft.EntityFrameworkCore;
using MinuteSheetFFC.Application.DTOs.Common;
using MinuteSheetFFC.Application.DTOs.Dashboard;
using MinuteSheetFFC.Application.Interfaces;
using MinuteSheetFFC.Domain.Enums;
using MinuteSheetFFC.Infrastructure.Data;

namespace MinuteSheetFFC.Api.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db) => _db = db;

    public async Task<ApiResponse<DashboardSummaryDto>> GetSummaryAsync(string userPNo)
    {
        var myRequests = _db.MinuteSheetRequests.Where(m => m.RequesterPNo == userPNo);
        var pendingActions = await _db.WorkflowStages
            .Where(s => s.ActionerPNo == userPNo && s.Status == StageStatus.Active)
            .CountAsync();

        var summary = new DashboardSummaryDto
        {
            MyTotalRequests = await myRequests.CountAsync(),
            MyPendingActions = pendingActions,
            DraftCount = await myRequests.CountAsync(m => m.Status == RequestStatus.Draft),
            InReviewCount = await myRequests.CountAsync(m => m.Status == RequestStatus.InReview),
            ApprovedCount = await myRequests.CountAsync(m => m.Status == RequestStatus.Approved),
            RejectedCount = await myRequests.CountAsync(m => m.Status == RequestStatus.Rejected),
            ReturnedCount = await myRequests.CountAsync(m => m.Status == RequestStatus.Returned),
            CancelledCount = await myRequests.CountAsync(m => m.Status == RequestStatus.Cancelled)
        };

        // Aging
        var threeDaysAgo = DateTime.UtcNow.AddDays(-3);
        summary.AgingItems = await _db.MinuteSheetRequests
            .Where(m => m.Status == RequestStatus.InReview && m.SubmittedAt < threeDaysAgo)
            .OrderBy(m => m.SubmittedAt)
            .Take(10)
            .Select(m => new AgingItemDto
            {
                MinuteSheetId = m.Id,
                ReferenceNumber = m.ReferenceNumber,
                Subject = m.Subject,
                CurrentActionerName = m.CurrentActioner != null ? m.CurrentActioner.Name : "N/A",
                DaysPending = (int)(DateTime.UtcNow - (m.SubmittedAt ?? DateTime.UtcNow)).TotalDays
            }).ToListAsync();

        return ApiResponse<DashboardSummaryDto>.Ok(summary);
    }
}
