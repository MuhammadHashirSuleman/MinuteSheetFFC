namespace MinuteSheetFFC.Application.DTOs.Employee;

public class EmployeeFilterDto
{
    public string? Search { get; set; }
    public int? DepartmentId { get; set; }
    public bool? IsActive { get; set; }
    public string? SortBy { get; set; }
    public string? SortDir { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class EmployeeListDto
{
    public string PNo { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Email { get; set; }
    public string Designation { get; set; } = null!;
    public string? DesignationShort { get; set; }
    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string? DepartmentShort { get; set; }
    public string? ManagerPNo { get; set; }
    public bool IsActive { get; set; }
}

public class EmployeeDetailDto : EmployeeListDto
{
    public string? OldPNo { get; set; }
    public string? Phone { get; set; }
    public string? CNIC { get; set; }
    public string? Gender { get; set; }
    public string? MaritalStatus { get; set; }
    public string? FatherName { get; set; }
    public DateTime? DOB { get; set; }
    public DateTime? HireDate { get; set; }
    public DateTime? LastPromotionDate { get; set; }
    public DateTime? RetirementDate { get; set; }
    public DateTime? LeavingDate { get; set; }
    public string? JobDescription { get; set; }
    public string? JobKey { get; set; }
    public string? PositionId { get; set; }
    public int? Grade { get; set; }
    public string? EmployeeGroup { get; set; }
    public string? EmployeeCategory { get; set; }
    public string? GroupId { get; set; }
    public string? GroupDesc { get; set; }
    public string? GroupShort { get; set; }
    public string? PAreaId { get; set; }
    public string? PAreaDesc { get; set; }
    public string? PSAreaId { get; set; }
    public string? PSAreaDesc { get; set; }
    public string? CostCenter { get; set; }
    public string? CostCenterDesc { get; set; }
    public decimal? AnnualLeaveBalance { get; set; }
    public string? ManagerName { get; set; }
}

public class ManagerChainItemDto
{
    public string PNo { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Designation { get; set; }
    public string? DepartmentName { get; set; }
    public int Level { get; set; }
    public bool IsActive { get; set; }
}
