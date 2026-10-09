using EkubCircle.Application.DTOs.Member;

namespace EkubCircle.Application.DTOs.Circle;

public class MemberDashboardDto
{
    public Guid CircleId { get; set; }

    public string CircleName { get; set; } = string.Empty;

    public decimal ContributionAmount { get; set; }

    public string MeetingLabel { get; set; } = string.Empty;

    public string CircleStatus { get; set; } = string.Empty;

    public int CurrentRoundNumber { get; set; }

    public string RoundStatus { get; set; } = string.Empty;

    public string ReceiverName { get; set; } = string.Empty;

    public decimal PotAmount { get; set; }

    public bool HasPaid { get; set; }

    public bool HasReceived { get; set; }

    public int PaidMemberCount { get; set; }

    public int TotalMemberCount { get; set; }

    // All members in this circle
    public List<MemberPaymentDto> Members { get; set; } = new();
}