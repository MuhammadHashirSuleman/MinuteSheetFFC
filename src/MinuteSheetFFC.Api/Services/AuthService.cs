using Microsoft.EntityFrameworkCore;
using MinuteSheetFFC.Application.DTOs.Auth;
using MinuteSheetFFC.Application.DTOs.Common;
using MinuteSheetFFC.Application.Interfaces;
using MinuteSheetFFC.Domain.Entities;
using MinuteSheetFFC.Infrastructure.Data;

namespace MinuteSheetFFC.Api.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly JwtTokenService _jwt;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext db, JwtTokenService jwt, IConfiguration config)
    {
        _db = db;
        _jwt = jwt;
        _config = config;
    }

    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request)
    {
        var user = await _db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return ApiResponse<AuthResponseDto>.Fail("Invalid username or password.");

        if (!user.IsActive)
            return ApiResponse<AuthResponseDto>.Fail("Account is deactivated.");

        if (user.IsLocked && user.LockoutEnd > DateTime.UtcNow)
            return ApiResponse<AuthResponseDto>.Fail("Account is locked. Try again later.");

        user.FailedLoginAttempts = 0;
        user.LastLoginAt = DateTime.UtcNow;
        user.IsLocked = false;
        user.LockoutEnd = null;

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var accessToken = _jwt.GenerateAccessToken(user.Id, user.Username, user.EmployeePNo, roles);
        var refreshToken = _jwt.GenerateRefreshToken();
        var refreshDays = int.Parse(_config["JwtSettings:RefreshTokenExpirationDays"]!);

        _db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshDays)
        });

        await _db.SaveChangesAsync();

        return ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(_config["JwtSettings:AccessTokenExpirationMinutes"]!)),
            User = MapProfile(user)
        });
    }

    public async Task<ApiResponse<bool>> RegisterAsync(RegisterRequestDto request)
    {
        if (request.Password != request.ConfirmPassword)
            return ApiResponse<bool>.Fail("Passwords do not match.");

        if (await _db.Users.AnyAsync(u => u.Username == request.Username))
            return ApiResponse<bool>.Fail("Username already exists.");

        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            return ApiResponse<bool>.Fail("Email already registered.");

        if (!string.IsNullOrEmpty(request.EmployeePNo))
        {
            var emp = await _db.Employees.FindAsync(request.EmployeePNo);
            if (emp == null) return ApiResponse<bool>.Fail("Employee PNo not found.");
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            EmployeePNo = request.EmployeePNo
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // Assign default Employee role
        var empRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "Employee");
        if (empRole != null)
        {
            _db.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = empRole.Id });
            await _db.SaveChangesAsync();
        }

        return ApiResponse<bool>.Ok(true, "Registration successful.");
    }

    public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var principal = _jwt.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null) return ApiResponse<AuthResponseDto>.Fail("Invalid token.");

        var userIdStr = principal.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
            return ApiResponse<AuthResponseDto>.Fail("Invalid token.");

        var storedToken = await _db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && rt.UserId == userId);

        if (storedToken == null || storedToken.RevokedAt != null || storedToken.ExpiresAt < DateTime.UtcNow)
            return ApiResponse<AuthResponseDto>.Fail("Invalid or expired refresh token.");

        storedToken.RevokedAt = DateTime.UtcNow;

        var user = await _db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return ApiResponse<AuthResponseDto>.Fail("User not found.");

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var newAccessToken = _jwt.GenerateAccessToken(user.Id, user.Username, user.EmployeePNo, roles);
        var newRefreshToken = _jwt.GenerateRefreshToken();
        var refreshDays = int.Parse(_config["JwtSettings:RefreshTokenExpirationDays"]!);

        storedToken.ReplacedByToken = newRefreshToken;
        _db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshDays)
        });

        await _db.SaveChangesAsync();

        return ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(_config["JwtSettings:AccessTokenExpirationMinutes"]!)),
            User = MapProfile(user)
        });
    }

    public async Task<ApiResponse<bool>> LogoutAsync(int userId)
    {
        var tokens = await _db.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync();
        foreach (var t in tokens) t.RevokedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "Logged out.");
    }

    public async Task<ApiResponse<UserProfileDto>> GetProfileAsync(int userId)
    {
        var user = await _db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return ApiResponse<UserProfileDto>.Fail("User not found.");
        return ApiResponse<UserProfileDto>.Ok(MapProfile(user));
    }

    public async Task<ApiResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDto request)
    {
        if (request.NewPassword != request.ConfirmNewPassword)
            return ApiResponse<bool>.Fail("New passwords do not match.");

        var user = await _db.Users.FindAsync(userId);
        if (user == null) return ApiResponse<bool>.Fail("User not found.");

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            return ApiResponse<bool>.Fail("Current password is incorrect.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.PasswordChangedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "Password changed.");
    }

    private static UserProfileDto MapProfile(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        EmployeePNo = user.EmployeePNo,
        EmployeeName = user.Employee?.Name,
        Designation = user.Employee?.Designation,
        Department = user.Employee?.DepartmentName,
        Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
    };
}
