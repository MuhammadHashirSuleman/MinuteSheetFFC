using System.Text.RegularExpressions;
using MinuteSheetFFC.Application.DTOs.Ai;
using MinuteSheetFFC.Application.DTOs.Common;
using MinuteSheetFFC.Application.Interfaces;

namespace MinuteSheetFFC.AiService;

public class MockAiMinuteSheetService : IAiMinuteSheetService
{
    public Task<ApiResponse<AiAnalysisDto>> AnalyzeAsync(AiAnalyzeRequestDto request)
    {
        var analysis = new AiAnalysisDto();
        var bodyLower = (request.Body ?? "").ToLower();
        var subjectLower = (request.Subject ?? "").ToLower();
        var combined = $"{subjectLower} {bodyLower}";

        var plainText = Regex.Replace(request.Body ?? "", "<[^>]+>", " ");
        plainText = Regex.Replace(plainText, @"\s+", " ").Trim();
        var sentences = plainText.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        analysis.Summary = string.Join(". ", sentences.Take(5).Select(s => s.Trim())) + ".";

        analysis.DetectedBudget = request.EstimatedBudget;

        if (Regex.IsMatch(combined, @"\b(urgent|immediately|asap|critical|emergency)\b"))
            analysis.Urgency = "Critical";
        else if (Regex.IsMatch(combined, @"\b(soon|priority|important)\b"))
            analysis.Urgency = "High";
        else
            analysis.Urgency = "Medium";

        var budget = request.EstimatedBudget ?? 0;
        analysis.RiskLevel = budget switch { > 500000 => "High", > 100000 => "Medium", _ => "Low" };

        analysis.MissingInformation = new List<string>();
        if (budget > 0 && !Regex.IsMatch(combined, @"\b(quot|vendor|supplier)\b"))
            analysis.MissingInformation.Add("Vendor quotation not attached");
        if (!Regex.IsMatch(combined, @"\b(date|timeline|deadline|deliver)\b"))
            analysis.MissingInformation.Add("Expected completion date not specified");

        analysis.RiskFlags = new List<string>();
        if (budget > 100000)
            analysis.RiskFlags.Add("Budget exceeds PKR 100,000 - requires additional approval levels");
        if (budget > 500000)
            analysis.RiskFlags.Add("High-value request - requires senior management approval");

        analysis.ReviewerChecklist = new List<string>
        {
            "Verify request aligns with departmental objectives",
            "Confirm requester has authority for this request type"
        };
        if (budget > 0)
        {
            analysis.ReviewerChecklist.Add("Verify budget against departmental allocation");
            analysis.ReviewerChecklist.Add("Check if similar expenditure was approved recently");
        }

        analysis.SuggestedRoute = $"Dynamic route with {(budget > 500000 ? 4 : budget > 100000 ? 3 : 2)} levels";

        return Task.FromResult(ApiResponse<AiAnalysisDto>.Ok(analysis));
    }

    public Task<ApiResponse<AiAnalysisDto>> GetAnalysisAsync(int minuteSheetId)
    {
        return Task.FromResult(ApiResponse<AiAnalysisDto>.Fail("No cached analysis. Run analysis first."));
    }
}
