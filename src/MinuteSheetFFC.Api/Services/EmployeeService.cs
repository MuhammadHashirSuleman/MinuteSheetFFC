using Microsoft.EntityFrameworkCore;
using MinuteSheetFFC.Application.DTOs.Common;
using MinuteSheetFFC.Application.DTOs.Employee;
using MinuteSheetFFC.Application.Interfaces;
using MinuteSheetFFC.Infrastructure.Data;
using MinuteSheetFFC.WorkflowEngine.Services;

namespace MinuteSheetFFC.Api.Services;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _db;
    private readonly HierarchyResolver _hierarchyResolver;

    public EmployeeService(AppDbContext db, HierarchyResolver hierarchyResolver)
    {
        _db = db;
        _hierarchyResolver = hierarchyResolver;
    }

    public async Task<PagedResponse<EmployeeListDto>> GetEmployeesAsync(EmployeeFilterDto filter)
    {
        var query = _db.Employees.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var s = filter.Search.ToLower();
            query = query.Where(e => e.Name.ToLower().Contains(s) || e.PNo.Contains(s) || (e.Email != null && e.Email.ToLower().Contains(s)) || e.Designation.ToLower().Contains(s));
        }
        if (filter.DepartmentId.HasValue) query = query.Where(e => e.DepartmentId == filter.DepartmentId.Value);
        if (filter.IsActive.HasValue) query = query.Where(e => e.IsActive == filter.IsActive.Value);

        var totalCount = await query.CountAsync();

        query = (filter.SortBy?.ToLower(), filter.SortDir?.ToLower()) switch
        {
            ("pno", "desc") => query.OrderByDescending(e => e.PNo),
            ("pno", _) => query.OrderBy(e => e.PNo),
            ("designation", "desc") => query.OrderByDescending(e => e.Designation),
            ("designation", _) => query.OrderBy(e => e.Designation),
            ("department", "desc") => query.OrderByDescending(e => e.DepartmentName),
            ("department", _) => query.OrderBy(e => e.DepartmentName),
            (_, "desc") => query.OrderByDescending(e => e.Name),
            _ => query.OrderBy(e => e.Name),
        };

        var items = await query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize)
            .Select(e => new EmployeeListDto
            {
                PNo = e.PNo, Name = e.Name, Email = e.Email, Designation = e.Designation,
                DesignationShort = e.DesignationShort, DepartmentId = e.DepartmentId,
                DepartmentName = e.DepartmentName, DepartmentShort = e.DepartmentShort,
                ManagerPNo = e.ManagerPNo, IsActive = e.IsActive
            }).ToListAsync();

        return new PagedResponse<EmployeeListDto> { Data = items, Page = filter.Page, PageSize = filter.PageSize, TotalCount = totalCount };
    }

    public async Task<ApiResponse<EmployeeDetailDto>> GetEmployeeAsync(string pno)
    {
        var e = await _db.Employees.Include(emp => emp.Manager).FirstOrDefaultAsync(emp => emp.PNo == pno);
        if (e == null) return ApiResponse<EmployeeDetailDto>.Fail("Employee not found.");
        return ApiResponse<EmployeeDetailDto>.Ok(new EmployeeDetailDto
        {
            PNo = e.PNo, Name = e.Name, Email = e.Email, Designation = e.Designation,
            DesignationShort = e.DesignationShort, DepartmentId = e.DepartmentId,
            DepartmentName = e.DepartmentName, DepartmentShort = e.DepartmentShort,
            ManagerPNo = e.ManagerPNo, IsActive = e.IsActive, OldPNo = e.OldPNo,
            Phone = e.Phone, CNIC = e.CNIC, Gender = e.Gender, MaritalStatus = e.MaritalStatus,
            FatherName = e.FatherName, DOB = e.DOB, HireDate = e.HireDate,
            LastPromotionDate = e.LastPromotionDate, RetirementDate = e.RetirementDate,
            LeavingDate = e.LeavingDate, JobDescription = e.JobDescription, JobKey = e.JobKey,
            PositionId = e.PositionId, Grade = e.Grade, EmployeeGroup = e.EmployeeGroup,
            EmployeeCategory = e.EmployeeCategory, GroupId = e.GroupId, GroupDesc = e.GroupDesc,
            GroupShort = e.GroupShort, PAreaId = e.PAreaId, PAreaDesc = e.PAreaDesc,
            PSAreaId = e.PSAreaId, PSAreaDesc = e.PSAreaDesc, CostCenter = e.CostCenter,
            CostCenterDesc = e.CostCenterDesc, AnnualLeaveBalance = e.AnnualLeaveBalance,
            ManagerName = e.Manager?.Name
        });
    }

    public async Task<ApiResponse<List<ManagerChainItemDto>>> GetManagerChainAsync(string pno)
    {
        var result = await _hierarchyResolver.ResolveManagerChainAsync(pno);
        var chain = result.Chain.Select(c => new ManagerChainItemDto
        {
            PNo = c.PNo, Name = c.Name, Designation = c.Designation,
            DepartmentName = c.DepartmentName, Level = c.Level, IsActive = c.IsActive
        }).ToList();
        return ApiResponse<List<ManagerChainItemDto>>.Ok(chain);
    }

    public async Task<List<EmployeeListDto>> SearchEmployeesAsync(string query)
    {
        var s = query.ToLower();
        return await _db.Employees.Where(e => e.IsActive && (e.Name.ToLower().Contains(s) || e.PNo.Contains(s)))
            .Take(20)
            .Select(e => new EmployeeListDto
            {
                PNo = e.PNo, Name = e.Name, Email = e.Email, Designation = e.Designation,
                DesignationShort = e.DesignationShort, DepartmentId = e.DepartmentId,
                DepartmentName = e.DepartmentName, DepartmentShort = e.DepartmentShort,
                ManagerPNo = e.ManagerPNo, IsActive = e.IsActive
            }).ToListAsync();
    }
}
