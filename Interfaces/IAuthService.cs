using DayOneAPI.Models.DTOs.Auth;

namespace DayOneAPI.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}
