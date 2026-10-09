namespace EkubCircle.Application.DTOs.Circle;

public class CircleDashboardDto
{
    public Guid CircleId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal ContributionAmount { get; set; }

    public string MeetingLabel { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public int CurrentRoundNumber { get; set; }

    public List<MemberPaymentDto> Members { get; set; } = new();

    public List<RoundInfoDto> Rounds { get; set; } = new();
}