using MinuteSheetFFC.Domain.Enums;

namespace MinuteSheetFFC.Domain.Entities;

public class WorkflowHistory
{
    public int Id { get; set; }
    public int MinuteSheetId { get; set; }
    public string ActionerPNo { get; set; } = null!;
    public string? ActionerName { get; set; }
    public WorkflowActionType Action { get; set; }
    public RequestStatus PreviousStatus { get; set; }
    public RequestStatus NewStatus { get; set; }
    public string? Remarks { get; set; }
    public int? StageOrder { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Navigation
    public MinuteSheetRequest MinuteSheet { get; set; } = null!;
}
