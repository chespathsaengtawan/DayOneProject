using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DayOneAPI.Interfaces;
using DayOneAPI.Models.DTOs.Auth;
using DayOneAPI.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DayOneAPI.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.EmployeeId == request.EmployeeId))
            throw new InvalidOperationException("รหัสพนักงานนี้ถูกลงทะเบียนแล้ว");

        // Password = StartDate formatted as ddMMyyyy
        var password = request.StartDate.ToString("ddMMyyyy");

        var user = new User
        {
            EmployeeId   = request.EmployeeId,
            FirstName    = request.FirstName,
            LastName     = request.LastName,
            StartDate    = request.StartDate,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return BuildResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.EmployeeId == request.EmployeeId)
            ?? throw new UnauthorizedAccessException("รหัสพนักงานหรือรหัสผ่านไม่ถูกต้อง");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("รหัสพนักงานหรือรหัสผ่านไม่ถูกต้อง");

        return BuildResponse(user);
    }

    private AuthResponse BuildResponse(User user)
    {
        var expiryHours = _config.GetValue<int>("Jwt:ExpiryHours", 8);
        var expiresAt   = DateTime.UtcNow.AddHours(expiryHours);
        var token       = GenerateToken(user, expiresAt);

        return new AuthResponse
        {
            Token     = token,
            ExpiresAt = expiresAt,
            User = new UserInfo
            {
                Id         = user.Id,
                EmployeeId = user.EmployeeId,
                FirstName  = user.FirstName,
                LastName   = user.LastName,
                StartDate  = user.StartDate
            }
        };
    }

    private string GenerateToken(User user, DateTime expiresAt)
    {
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,  user.Id.ToString()),
            new Claim("employee_id",                user.EmployeeId),
            new Claim(JwtRegisteredClaimNames.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(JwtRegisteredClaimNames.Jti,  Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer:             _config["Jwt:Issuer"],
            audience:           _config["Jwt:Audience"],
            claims:             claims,
            expires:            expiresAt,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
