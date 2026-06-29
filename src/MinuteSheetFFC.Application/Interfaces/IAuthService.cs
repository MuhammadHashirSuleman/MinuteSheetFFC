using MinuteSheetFFC.Application.DTOs.Auth;
using MinuteSheetFFC.Application.DTOs.Common;

namespace MinuteSheetFFC.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request);
    Task<ApiResponse<bool>> RegisterAsync(RegisterRequestDto request);
    Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task<ApiResponse<bool>> LogoutAsync(int userId);
    Task<ApiResponse<UserProfileDto>> GetProfileAsync(int userId);
    Task<ApiResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDto request);
}
