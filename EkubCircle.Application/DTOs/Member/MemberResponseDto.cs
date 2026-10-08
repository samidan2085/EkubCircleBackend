namespace EkubCircle.Application.DTOs.Member;

public class MemberResponseDto
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime JoinedAt { get; set; }
}