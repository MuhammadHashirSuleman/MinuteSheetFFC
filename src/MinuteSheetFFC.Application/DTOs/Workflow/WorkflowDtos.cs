namespace MinuteSheetFFC.Application.DTOs.Workflow;

public class WorkflowStageDto
{
    public int Id { get; set; }
    public int StageOrder { get; set; }
    public string ActionerPNo { get; set; } = null!;
    public string? ActionerName { get; set; }
    public string? ActionerDesignation { get; set; }
    public string? ActionType { get; set; }
    public string? Status { get; set; }
    public string? Action { get; set; }
    public string? Remarks { get; set; }
    public DateTime? ActionedAt { get; set; }
    public string? Source { get; set; }
}

public class WorkflowHistoryDto
{
    public int Id { get; set; }
    public string ActionerPNo { get; set; } = null!;
    public string? ActionerName { get; set; }
    public string? Action { get; set; }
    public string? PreviousStatus { get; set; }
    public string? NewStatus { get; set; }
    public string? Remarks { get; set; }
    public int? StageOrder { get; set; }
    public DateTime Timestamp { get; set; }
}

public class RoutePreviewDto
{
    public bool IsValid { get; set; }
    public List<RouteStepDto> Steps { get; set; } = new();
    public List<RouteWarningDto> Warnings { get; set; } = new();
}

public class RouteStepDto
{
    public int StageOrder { get; set; }
    public string ActionerPNo { get; set; } = null!;
    public string? ActionerName { get; set; }
    public string? ActionerDesignation { get; set; }
    public string? ActionType { get; set; }
    public string? Source { get; set; }
}

public class RouteWarningDto
{
    public string Message { get; set; } = null!;
    public string? Level { get; set; }
}

public class WorkflowActionDto
{
    public string ActionerPNo { get; set; } = null!;
    public string ActionType { get; set; } = null!;
    public string? Remarks { get; set; }
}

public class WorkflowActionResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public string? NewStatus { get; set; }
    public string? NextActionerPNo { get; set; }
}

public class PendingActionDto
{
    public int MinuteSheetId { get; set; }
    public string ReferenceNumber { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string? RequesterPNo { get; set; }
    public string? RequesterName { get; set; }
    public string? ActionType { get; set; }
    public int StageOrder { get; set; }
    public DateTime? SubmittedAt { get; set; }
}
