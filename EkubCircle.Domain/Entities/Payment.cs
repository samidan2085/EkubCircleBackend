namespace EkubCircle.Domain.Entities;

public class Payment
{
    public Guid Id { get; set; }

    public Guid RoundId { get; set; }

    public string MemberId { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime PaidAt { get; set; } = DateTime.UtcNow;

    public Round? Round { get; set; }

    public ApplicationUser? Member { get; set; }
}