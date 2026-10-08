namespace EkubCircle.Domain.Entities;

public class CircleMember
{
    public Guid Id { get; set; }

    public Guid CircleId { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int Position { get; set; }

    public bool HasReceived { get; set; }

    public Circle? Circle { get; set; }

    public ApplicationUser? User { get; set; }
}