namespace EkubCircle.Application.DTOs.Auth;

public class CreateMemberDto
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string TemporaryPassword { get; set; } = string.Empty;
}