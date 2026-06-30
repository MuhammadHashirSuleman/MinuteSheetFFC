using System.ComponentModel.DataAnnotations;

namespace MinuteSheetFFC.Services;

public enum MinuteSheetStatus
{
    Draft,
    Submitted,
    InReview,
    Approved,
    Rejected,
    Returned,
    Cancelled,
    Resubmitted,
    Marked
}

public enum MinuteSheetPriority
{
    Low,
    Medium,
    High,
    Urgent
}

public enum WorkflowMode
{
    Manual,
    Dynamic,
    Hybrid
}

public sealed class EmployeeProfile
{
    public string PNo { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
}

public sealed class MinuteSheetAttachment
{
    public string FileName { get; set; } = string.Empty;
    public string SizeLabel { get; set; } = string.Empty;
    public DateTimeOffset UploadedAt { get; set; }
}

public sealed class WorkflowStage
{
    public int Order { get; set; }
    public string ActionerPNo { get; set; } = string.Empty;
    public string ActionerName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public MinuteSheetStatus Status { get; set; }
    public string Remarks { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsUpcoming { get; set; }
}

public sealed class MinuteSheetRecord
{
    public int Id { get; set; }
    public string MinuteSheetId { get; set; } = string.Empty;
    public string ReferenceNo { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; }
    public MinuteSheetStatus Status { get; set; }
    public string CurrentActionerPNo { get; set; } = string.Empty;
    public string CurrentActionerName { get; set; } = string.Empty;
    public string RequesterPNo { get; set; } = string.Empty;
    public string RequesterName { get; set; } = string.Empty;
    public string RequesterDepartment { get; set; } = string.Empty;
    public MinuteSheetPriority Priority { get; set; }
    public decimal? EstimatedBudget { get; set; }
    public bool Confidential { get; set; }
    public WorkflowMode WorkflowMode { get; set; }
    public string DescriptionHtml { get; set; } = string.Empty;
    public bool SharedWithCurrentUser { get; set; }
    public int DaysDelayed { get; set; }
    public List<MinuteSheetAttachment> Attachments { get; } = new();
    public List<WorkflowStage> WorkflowStages { get; } = new();
}

public sealed class CreateMinuteSheetModel : IValidatableObject
{
    [Required]
    [StringLength(160, MinimumLength = 8)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = "HR & Recruitment";

    [Range(typeof(decimal), "1", "999999999", ErrorMessage = "Enter a valid estimated budget.")]
    public decimal? EstimatedBudget { get; set; }

    [Required]
    public MinuteSheetPriority Priority { get; set; } = MinuteSheetPriority.Low;

    public bool Confidential { get; set; }

    [Required]
    public WorkflowMode WorkflowMode { get; set; } = WorkflowMode.Manual;

    [Required]
    [MinLength(20, ErrorMessage = "Description must include enough detail for approval.")]
    public string DescriptionHtml { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (IsFinancialType(Type) && EstimatedBudget is null)
        {
            yield return new ValidationResult(
                "Estimated budget is required for financial minute sheets.",
                new[] { nameof(EstimatedBudget) });
        }
    }

    public static bool IsFinancialType(string? type) =>
        string.Equals(type, "Financial", StringComparison.OrdinalIgnoreCase)
        || string.Equals(type, "Financial & Procurement", StringComparison.OrdinalIgnoreCase);
}

public sealed class WorkflowDecisionModel
{
    [Required]
    [MinLength(5, ErrorMessage = "Please enter decision remarks.")]
    public string Remarks { get; set; } = string.Empty;
}

public sealed class DashboardSnapshot
{
    public int MyRequests { get; set; }
    public int PendingActions { get; set; }
    public int Drafts { get; set; }
    public int Approved { get; set; }
    public IReadOnlyList<MinuteSheetRecord> PendingItems { get; set; } = Array.Empty<MinuteSheetRecord>();
    public IReadOnlyList<MinuteSheetRecord> RecentItems { get; set; } = Array.Empty<MinuteSheetRecord>();
    public IReadOnlyList<MinuteSheetRecord> DelayedItems { get; set; } = Array.Empty<MinuteSheetRecord>();
}

public static class MinuteSheetDisplay
{
    public static string ToDisplayName(this MinuteSheetStatus status) =>
        status switch
        {
            MinuteSheetStatus.InReview => "In Review",
            _ => status.ToString()
        };

    public static string ToDisplayName(this MinuteSheetPriority priority) => priority.ToString();

    public static string ToDisplayName(this WorkflowMode mode) => mode.ToString();

    public static string StatusCss(this MinuteSheetStatus status) =>
        status switch
        {
            MinuteSheetStatus.Draft => "status-draft",
            MinuteSheetStatus.Submitted => "status-submitted",
            MinuteSheetStatus.InReview => "status-review",
            MinuteSheetStatus.Approved => "status-approved",
            MinuteSheetStatus.Rejected => "status-rejected",
            MinuteSheetStatus.Returned => "status-returned",
            MinuteSheetStatus.Cancelled => "status-cancelled",
            MinuteSheetStatus.Resubmitted => "status-resubmitted",
            MinuteSheetStatus.Marked => "status-marked",
            _ => "status-draft"
        };

    public static string PriorityCss(this MinuteSheetPriority priority) =>
        priority switch
        {
            MinuteSheetPriority.Low => "priority-low",
            MinuteSheetPriority.Medium => "priority-medium",
            MinuteSheetPriority.High => "priority-high",
            MinuteSheetPriority.Urgent => "priority-urgent",
            _ => "priority-low"
        };

    public static string FormatCurrency(decimal? amount) =>
        amount.HasValue
            ? string.Format(System.Globalization.CultureInfo.InvariantCulture, "${0:N2} USD", amount.Value)
            : "Not Applicable";
}
