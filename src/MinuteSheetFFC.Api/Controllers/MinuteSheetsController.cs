using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinuteSheetFFC.Application.DTOs.MinuteSheet;
using MinuteSheetFFC.Application.Interfaces;

namespace MinuteSheetFFC.Api.Controllers;

[ApiController]
[Route("api/v1/minute-sheets")]
[Authorize]
public class MinuteSheetsController : ControllerBase
{
    private readonly IMinuteSheetService _service;

    public MinuteSheetsController(IMinuteSheetService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] MinuteSheetFilterDto filter)
        => Ok(await _service.GetMinuteSheetsAsync(filter));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _service.GetMinuteSheetAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMinuteSheetDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return result.Success ? CreatedAtAction(nameof(Get), new { id = result.Data!.Id }, result) : BadRequest(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMinuteSheetDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:int}/submit")]
    public async Task<IActionResult> Submit(int id, [FromQuery] string requesterPNo)
    {
        var result = await _service.SubmitAsync(id, requesterPNo);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id, [FromQuery] string requesterPNo)
    {
        var result = await _service.CancelAsync(id, requesterPNo);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:int}/resubmit")]
    public async Task<IActionResult> Resubmit(int id, [FromQuery] string requesterPNo)
    {
        var result = await _service.ResubmitAsync(id, requesterPNo);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
