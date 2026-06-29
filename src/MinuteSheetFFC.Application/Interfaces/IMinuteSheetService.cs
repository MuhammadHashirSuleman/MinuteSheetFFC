using MinuteSheetFFC.Application.DTOs.Common;
using MinuteSheetFFC.Application.DTOs.MinuteSheet;

namespace MinuteSheetFFC.Application.Interfaces;

public interface IMinuteSheetService
{
    Task<PagedResponse<MinuteSheetListDto>> GetMinuteSheetsAsync(MinuteSheetFilterDto filter);
    Task<ApiResponse<MinuteSheetDetailDto>> GetMinuteSheetAsync(int id);
    Task<ApiResponse<MinuteSheetDetailDto>> CreateAsync(CreateMinuteSheetDto dto);
    Task<ApiResponse<MinuteSheetDetailDto>> UpdateAsync(int id, UpdateMinuteSheetDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
    Task<ApiResponse<MinuteSheetDetailDto>> SubmitAsync(int id, string requesterPNo);
    Task<ApiResponse<bool>> CancelAsync(int id, string requesterPNo);
    Task<ApiResponse<MinuteSheetDetailDto>> ResubmitAsync(int id, string requesterPNo);
}
