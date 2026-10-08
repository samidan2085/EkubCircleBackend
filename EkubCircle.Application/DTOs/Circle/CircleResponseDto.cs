namespace EkubCircle.Application.DTOs.Circle;

public class CircleResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal ContributionAmount { get; set; }

    public string MeetingLabel { get; set; } = string.Empty;

    public string OrganizerId { get; set; } = string.Empty;
}