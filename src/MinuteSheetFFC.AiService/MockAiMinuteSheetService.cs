using MinuteSheetFFC.Application.DTOs.Ai;
using MinuteSheetFFC.Application.DTOs.Common;
using MinuteSheetFFC.Application.Interfaces;

namespace MinuteSheetFFC.AiService;

public class MockAiMinuteSheetService : IAiMinuteSheetService
{
    public Task<ApiResponse<AiAnalysisResultDto>> AnalyzeAsync(AiAnalyzeRequestDto request)
    {
        var result = new AiAnalysisResultDto
        {
            MinuteSheetId = request.MinuteSheetId,
            Summary = $"AI analysis for: {request.Subject ?? "Untitled"}",
            Suggestions = new List<string>
            {
                "Consider adding more detail to the request body.",
                "Ensure budget estimates align with departmental limits.",
                "Attach supporting documents for faster approval."
            },
            RiskLevel = request.EstimatedBudget > 100000 ? "High" : request.EstimatedBudget > 10000 ? "Medium" : "Low",
            RecommendedPriority = request.EstimatedBudget > 50000 ? "High" : "Normal",
            AnalyzedAt = DateTime.UtcNow
        };

        return Task.FromResult(ApiResponse<AiAnalysisResultDto>.Ok(result));
    }

    public Task<ApiResponse<AiAnalysisResultDto>> GetAnalysisAsync(int minuteSheetId)
    {
        var result = new AiAnalysisResultDto
        {
            MinuteSheetId = minuteSheetId,
            Summary = "Cached AI analysis (mock).",
            Suggestions = new List<string> { "No cached analysis available. Run a new analysis." },
            RiskLevel = "Unknown",
            RecommendedPriority = "Normal",
            AnalyzedAt = DateTime.UtcNow
        };

        return Task.FromResult(ApiResponse<AiAnalysisResultDto>.Ok(result));
    }
}
