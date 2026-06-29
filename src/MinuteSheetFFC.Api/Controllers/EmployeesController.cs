using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinuteSheetFFC.Application.DTOs.Employee;
using MinuteSheetFFC.Application.Interfaces;

namespace MinuteSheetFFC.Api.Controllers;

[ApiController]
[Route("api/v1/employees")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService) => _employeeService = employeeService;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] EmployeeFilterDto filter)
        => Ok(await _employeeService.GetEmployeesAsync(filter));

    [HttpGet("{pno}")]
    public async Task<IActionResult> Get(string pno)
    {
        var result = await _employeeService.GetEmployeeAsync(pno);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("{pno}/manager-chain")]
    public async Task<IActionResult> GetManagerChain(string pno)
        => Ok(await _employeeService.GetManagerChainAsync(pno));

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
        => Ok(await _employeeService.SearchEmployeesAsync(q ?? ""));
}
