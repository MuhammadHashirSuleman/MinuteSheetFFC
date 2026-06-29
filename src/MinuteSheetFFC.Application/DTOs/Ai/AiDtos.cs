namespace MinuteSheetFFC.Application.DTOs.Ai;

public class AiAnalyzeRequestDto
{
    public string Subject { get; set; } = "";
    public string Body { get; set; } = "";
    public decimal? EstimatedBudget { get; set; }
    public string? RequestType { get; set; }
}

public class AiAnalysisDto
{
    public string? Summary { get; set; }
    public decimal? DetectedBudget { get; set; }
    public string? Urgency { get; set; }
    public string? RiskLevel { get; set; }
    public List<string>? MissingInformation { get; set; }
    public List<string>? RiskFlags { get; set; }
    public List<string>? ReviewerChecklist { get; set; }
    public string? SuggestedRoute { get; set; }
}
