using MinuteSheetFFC.Application.DTOs.Common;
using MinuteSheetFFC.Application.DTOs.Employee;

namespace MinuteSheetFFC.Application.Interfaces;

public interface IEmployeeService
{
    Task<PagedResponse<EmployeeListDto>> GetEmployeesAsync(EmployeeFilterDto filter);
    Task<ApiResponse<EmployeeDetailDto>> GetEmployeeAsync(string pno);
    Task<ApiResponse<List<ManagerChainItemDto>>> GetManagerChainAsync(string pno);
    Task<List<EmployeeListDto>> SearchEmployeesAsync(string query);
}
