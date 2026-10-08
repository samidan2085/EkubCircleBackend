using EkubCircle.Application.DTOs.Auth;

namespace EkubCircle.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);

    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);

    Task ChangePasswordAsync(
        string userId,
        ChangePasswordRequestDto request);
}