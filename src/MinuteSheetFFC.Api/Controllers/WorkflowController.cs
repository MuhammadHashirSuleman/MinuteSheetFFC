using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinuteSheetFFC.Application.DTOs.Workflow;
using MinuteSheetFFC.Application.Interfaces;

namespace MinuteSheetFFC.Api.Controllers;

[ApiController]
[Route("api/v1/workflow")]
[Authorize]
public class WorkflowController : ControllerBase
{
    private readonly IWorkflowEngineService _service;

    public WorkflowController(IWorkflowEngineService service) => _service = service;

    [HttpPost("{minuteSheetId:int}/preview-route")]
    public async Task<IActionResult> PreviewRoute(int minuteSheetId)
    {
        var result = await _service.GenerateRoutePreviewAsync(minuteSheetId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{minuteSheetId:int}/action")]
    public async Task<IActionResult> PerformAction(int minuteSheetId, [FromBody] WorkflowActionDto action)
    {
        var result = await _service.ProcessActionAsync(minuteSheetId, action);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("my-pending-actions")]
    public async Task<IActionResult> GetPendingActions([FromQuery] string actionerPNo)
    {
        var result = await _service.GetPendingActionsAsync(actionerPNo);
        return Ok(result);
    }

    [HttpGet("rules")]
    public async Task<IActionResult> GetRules([FromServices] MinuteSheetFFC.Infrastructure.Data.AppDbContext db)
    {
        var rules = await db.WorkflowRules.Include(r => r.RequestType).Where(r => r.IsActive).ToListAsync();
        return Ok(rules.Select(r => new { r.Id, RequestType = r.RequestType.Name, r.BudgetFrom, r.BudgetTo, r.RequiredManagerLevels, r.RequiresFinanceReview, FallbackBehavior = r.FallbackBehavior.ToString() }));
    }
}
