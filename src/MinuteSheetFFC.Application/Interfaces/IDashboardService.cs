using MinuteSheetFFC.Application.DTOs.Common;
using MinuteSheetFFC.Application.DTOs.Dashboard;

namespace MinuteSheetFFC.Application.Interfaces;

public interface IDashboardService
{
    Task<ApiResponse<DashboardSummaryDto>> GetSummaryAsync(string userPNo);
}
