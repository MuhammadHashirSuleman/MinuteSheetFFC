namespace MinuteSheetFFC.Application.DTOs.MinuteSheet;

public class MinuteSheetFilterDto
{
    public string? Search { get; set; }
    public string? Status { get; set; }
    public string? RequesterPNo { get; set; }
    public string? CurrentActionerPNo { get; set; }
    public string? Priority { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public decimal? BudgetMin { get; set; }
    public decimal? BudgetMax { get; set; }
    public string? SortBy { get; set; }
    public string? SortDir { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class MinuteSheetListDto
{
    public int Id { get; set; }
    public string ReferenceNumber { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string? RequestTypeCode { get; set; }
    public string? RequestTypeName { get; set; }
    public decimal? EstimatedBudget { get; set; }
    public string? Priority { get; set; }
    public bool IsConfidential { get; set; }
    public string? WorkflowMode { get; set; }
    public string? Status { get; set; }
    public string? RequesterPNo { get; set; }
    public string? RequesterName { get; set; }
    public string? CurrentActionerPNo { get; set; }
    public string? CurrentActionerName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
}

public class MinuteSheetDetailDto : MinuteSheetListDto
{
    public string? Body { get; set; }
    public int? CurrentStageOrder { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<MinuteSheetFFC.Application.DTOs.Workflow.WorkflowStageDto> Stages { get; set; } = new();
    public List<MinuteSheetFFC.Application.DTOs.Workflow.WorkflowHistoryDto> History { get; set; } = new();
    public List<AttachmentDto> Attachments { get; set; } = new();
}

public class CreateMinuteSheetDto
{
    public string Subject { get; set; } = null!;
    public string? Body { get; set; }
    public int RequestTypeId { get; set; }
    public decimal? EstimatedBudget { get; set; }
    public string? Priority { get; set; }
    public bool IsConfidential { get; set; }
    public string? WorkflowMode { get; set; }
    public string RequesterPNo { get; set; } = null!;
}

public class UpdateMinuteSheetDto
{
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public int? RequestTypeId { get; set; }
    public decimal? EstimatedBudget { get; set; }
    public string? Priority { get; set; }
    public bool? IsConfidential { get; set; }
    public string? WorkflowMode { get; set; }
}

public class AttachmentDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = null!;
    public string? FileType { get; set; }
    public long FileSize { get; set; }
    public string? UploadedByPNo { get; set; }
    public DateTime UploadedAt { get; set; }
}
