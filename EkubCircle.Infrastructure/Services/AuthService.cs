using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EkubCircle.Application.DTOs.Auth;
using EkubCircle.Application.Interfaces;
using EkubCircle.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EkubCircle.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterAsync(
        RegisterRequestDto request)
    {
        ValidateRegistration(request);

        var email = request.Email.Trim().ToLowerInvariant();

        var existing = await _userManager.FindByEmailAsync(email);

        if (existing != null)
            throw new InvalidOperationException(
                "An account with this email already exists.");

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = email,
            Email = email,
            FullName = request.FullName.Trim(),
            MustChangePassword = false
        };

        var result = await _userManager.CreateAsync(
            user,
            request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                "; ",
                result.Errors.Select(x => x.Description));

            throw new InvalidOperationException(errors);
        }

        await _userManager.AddToRoleAsync(user, "Organizer");

        return await BuildResponseAsync(user, "Organizer");
    }

    public async Task<AuthResponseDto> LoginAsync(
        LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException(
                "Username and password are required.");
        }

        var username = request.Email.Trim();

        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            // Also allow users to log in with their email address.
            user = await _userManager.FindByEmailAsync(username);
        }

        if (user == null)
            throw new UnauthorizedAccessException(
                "Invalid username or password.");

        var valid = await _userManager.CheckPasswordAsync(
            user,
            request.Password);

        if (!valid)
            throw new UnauthorizedAccessException(
                "Invalid username or password.");

        var roles = await _userManager.GetRolesAsync(user);

        var role = roles.FirstOrDefault() ?? "Member";

        return await BuildResponseAsync(user, role);
    }

    public async Task ChangePasswordAsync(
        string userId,
        ChangePasswordRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.CurrentPassword))
            throw new ArgumentException(
                "Current password is required.");

        if (string.IsNullOrWhiteSpace(request.NewPassword))
            throw new ArgumentException(
                "New password is required.");

        if (request.NewPassword.Length < 6)
            throw new ArgumentException(
                "New password must be at least 6 characters.");

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            throw new KeyNotFoundException("User not found.");

        var result = await _userManager.ChangePasswordAsync(
            user,
            request.CurrentPassword,
            request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                "; ",
                result.Errors.Select(x => x.Description));

            throw new InvalidOperationException(errors);
        }

        user.MustChangePassword = false;

        await _userManager.UpdateAsync(user);
    }

    private async Task<AuthResponseDto> BuildResponseAsync(
        ApplicationUser user,
        string role)
    {
        var key = _configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException(
                "JWT key is missing. Check Jwt:Key in appsettings.json.");
        }

        var issuer = _configuration["Jwt:Issuer"];

        if (string.IsNullOrWhiteSpace(issuer))
        {
            throw new InvalidOperationException(
                "JWT issuer is missing. Check Jwt:Issuer.");
        }

        var audience = _configuration["Jwt:Audience"];

        if (string.IsNullOrWhiteSpace(audience))
        {
            throw new InvalidOperationException(
                "JWT audience is missing. Check Jwt:Audience.");
        }

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddHours(12),
            signingCredentials: credentials);

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler()
                .WriteToken(token),

            UserId = user.Id,

            FullName = user.FullName,

            Email = user.Email ?? string.Empty,

            Role = role,

            MustChangePassword = user.MustChangePassword
        };
    }

    private static void ValidateRegistration(
        RegisterRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
            throw new ArgumentException(
                "Full name is required.");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException(
                "Email is required.");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException(
                "Password is required.");

        if (request.Password.Length < 6)
            throw new ArgumentException(
                "Password must be at least 6 characters.");
    }
}