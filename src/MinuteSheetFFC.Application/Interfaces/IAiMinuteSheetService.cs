using MinuteSheetFFC.Application.DTOs.Ai;
using MinuteSheetFFC.Application.DTOs.Common;

namespace MinuteSheetFFC.Application.Interfaces;

public interface IAiMinuteSheetService
{
    Task<ApiResponse<AiAnalysisDto>> AnalyzeAsync(AiAnalyzeRequestDto request);
    Task<ApiResponse<AiAnalysisDto>> GetAnalysisAsync(int minuteSheetId);
}
