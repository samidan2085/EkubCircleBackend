namespace EkubCircle.Application.DTOs.Circle;

public class CircleDetailDto
{
    public string Name { get; set; } = string.Empty;

    public decimal ContributionAmount { get; set; }

    public string MeetingLabel { get; set; } = string.Empty;

    // Do not ask the user for this.
    // It comes from the JWT.
}

public class MemberPaymentDto
{
    public string MemberId { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public bool HasPaid { get; set; }

    public bool HasReceived { get; set; }
}

public class RoundInfoDto
{
    public int RoundNumber { get; set; }

    public string Status { get; set; } = string.Empty;

    public string DesignatedReceiver { get; set; } = string.Empty;
}