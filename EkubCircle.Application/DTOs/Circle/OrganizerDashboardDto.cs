namespace EkubCircle.Application.DTOs.Circle;

public class OrganizerDashboardDto
{
    public Guid CircleId { get; set; }
    public string CircleName { get; set; } = string.Empty;
    public decimal ContributionAmount { get; set; }
    public string MeetingLabel { get; set; } = string.Empty;
    public string CircleStatus { get; set; } = string.Empty;

    public int CurrentRoundNumber { get; set; }
    public decimal CurrentRoundFund { get; set; }
    public decimal TotalFundValue { get; set; }

    public int ActiveMembers { get; set; }
    public int TotalMembers { get; set; }
    public int CurrentRoundPaidMembers { get; set; }
    public int CurrentRoundTotalMembers { get; set; }

    public List<RoundFundPerformanceDto> FundPerformance { get; set; }
        = new();

    public List<MemberTransactionDto> Transactions { get; set; }
        = new();
}

public class RoundFundPerformanceDto
{
    public int RoundNumber { get; set; }
    public decimal CollectedAmount { get; set; }
    public decimal ExpectedAmount { get; set; }
    public int PaidMembers { get; set; }
    public int TotalMembers { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class MemberTransactionDto
{
    public string MemberId { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public int RoundNumber { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }
}