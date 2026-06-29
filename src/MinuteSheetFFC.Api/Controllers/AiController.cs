using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinuteSheetFFC.Application.DTOs.Ai;
using MinuteSheetFFC.Application.Interfaces;

namespace MinuteSheetFFC.Api.Controllers;

[ApiController]
[Route("api/v1/ai")]
[Authorize]
public class AiController : ControllerBase
{
    private readonly IAiMinuteSheetService _aiService;

    public AiController(IAiMinuteSheetService aiService) => _aiService = aiService;

    [HttpPost("analyze")]
    public async Task<IActionResult> Analyze([FromBody] AiAnalyzeRequestDto request)
    {
        var result = await _aiService.AnalyzeAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("analysis/{minuteSheetId:int}")]
    public async Task<IActionResult> GetAnalysis(int minuteSheetId)
    {
        var result = await _aiService.GetAnalysisAsync(minuteSheetId);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
