using EkubCircle.Domain.Enums;

namespace EkubCircle.Domain.Entities;

public class Round
{
    public Guid Id { get; set; }

    public Guid CircleId { get; set; }

    public int RoundNumber { get; set; }

    public string ReceiverId { get; set; } = string.Empty;

    public decimal ContributionAmount { get; set; }

    public decimal PotAmount { get; set; }

    public RoundStatus Status { get; set; } = RoundStatus.Open;

    public DateTime? PaidOutAt { get; set; }

    public Circle? Circle { get; set; }

    public ApplicationUser? Receiver { get; set; }

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}