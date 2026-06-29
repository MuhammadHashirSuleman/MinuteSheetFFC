using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinuteSheetFFC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MinuteSheetFFC.Api.Controllers;

[ApiController]
[Route("api/v1/request-types")]
[Authorize]
public class RequestTypesController : ControllerBase
{
    private readonly AppDbContext _db;
    public RequestTypesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _db.RequestTypes.Where(r => r.IsActive).Select(r => new { r.Id, r.Code, r.Name, r.Description }).ToListAsync());
}
