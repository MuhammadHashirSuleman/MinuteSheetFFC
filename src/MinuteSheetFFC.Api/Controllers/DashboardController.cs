using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinuteSheetFFC.Application.Interfaces;

namespace MinuteSheetFFC.Api.Controllers;

[ApiController]
[Route("api/v1/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service) => _service = service;

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] string userPNo)
    {
        var result = await _service.GetSummaryAsync(userPNo);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
