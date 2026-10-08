using EkubCircle.Domain.Enums;

namespace EkubCircle.Domain.Entities;

public class Circle
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal ContributionAmount { get; set; }

    public int MemberCount { get; set; }

    public string MeetingLabel { get; set; } = string.Empty;

    public string OrganizerId { get; set; } = string.Empty;

    public CircleStatus Status { get; set; } = CircleStatus.Active;

    public int CurrentRoundNumber { get; set; } = 1;

    public int CurrentReceiverPosition { get; set; } = 1;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser? Organizer { get; set; }

    public ICollection<CircleMember> Members { get; set; } = new List<CircleMember>();

    public ICollection<Round> Rounds { get; set; } = new List<Round>();
}