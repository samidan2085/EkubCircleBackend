namespace EkubCircle.Application.DTOs.Round;

public class RoundResponseDto
{
    public Guid Id { get; set; }

    public Guid CircleId { get; set; }

    public int RoundNumber { get; set; }

    public string ReceiverId { get; set; } = string.Empty;

    public decimal ContributionAmount { get; set; }

    public decimal PotAmount { get; set; }

    public int PaidMemberCount { get; set; }

    public int TotalMemberCount { get; set; }

    public bool IsCompleted { get; set; }
}