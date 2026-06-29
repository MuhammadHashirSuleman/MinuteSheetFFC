using MinuteSheetFFC.Domain.Enums;

namespace MinuteSheetFFC.Domain.Entities;

public class MinuteSheetRequest
{
    public int Id { get; set; }
    public string ReferenceNumber { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string? Body { get; set; }
    public int RequestTypeId { get; set; }
    public decimal? EstimatedBudget { get; set; }
    public Priority Priority { get; set; } = Priority.Normal;
    public bool IsConfidential { get; set; }
    public WorkflowMode WorkflowMode { get; set; } = WorkflowMode.Dynamic;
    public RequestStatus Status { get; set; } = RequestStatus.Draft;
    public string RequesterPNo { get; set; } = null!;
    public string? CurrentActionerPNo { get; set; }
    public int? CurrentStageOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Navigation
    public Employee Requester { get; set; } = null!;
    public Employee? CurrentActioner { get; set; }
    public RequestType RequestType { get; set; } = null!;
    public ICollection<WorkflowStage> Stages { get; set; } = new List<WorkflowStage>();
    public ICollection<WorkflowHistory> History { get; set; } = new List<WorkflowHistory>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}
