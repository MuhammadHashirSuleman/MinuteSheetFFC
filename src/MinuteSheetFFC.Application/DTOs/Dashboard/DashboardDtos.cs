namespace MinuteSheetFFC.Application.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public int MyTotalRequests { get; set; }
    public int MyPendingActions { get; set; }
    public int DraftCount { get; set; }
    public int InReviewCount { get; set; }
    public int ApprovedCount { get; set; }
    public int RejectedCount { get; set; }
    public int ReturnedCount { get; set; }
    public int CancelledCount { get; set; }
    public List<AgingItemDto> AgingItems { get; set; } = new();
}

public class AgingItemDto
{
    public int MinuteSheetId { get; set; }
    public string ReferenceNumber { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string? CurrentActionerName { get; set; }
    public int DaysPending { get; set; }
}
