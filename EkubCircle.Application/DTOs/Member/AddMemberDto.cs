namespace EkubCircle.Application.DTOs.Member;

public class AddMemberDto
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string TemporaryPassword { get; set; } = string.Empty;
}