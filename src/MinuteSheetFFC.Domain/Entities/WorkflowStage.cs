using MinuteSheetFFC.Domain.Enums;

namespace MinuteSheetFFC.Domain.Entities;

public class WorkflowStage
{
    public int Id { get; set; }
    public int MinuteSheetId { get; set; }
    public int StageOrder { get; set; }
    public string ActionerPNo { get; set; } = null!;
    public string? ActionerName { get; set; }
    public string? ActionerDesignation { get; set; }
    public StageActionType ActionType { get; set; }
    public StageStatus Status { get; set; } = StageStatus.Pending;
    public StageAction? Action { get; set; }
    public string? Remarks { get; set; }
    public DateTime? ActionedAt { get; set; }
    public StageSource Source { get; set; } = StageSource.Auto;

    // Navigation
    public MinuteSheetRequest MinuteSheet { get; set; } = null!;
}
