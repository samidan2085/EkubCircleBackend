namespace EkubCircle.Application.DTOs.Round;

public class TogglePaymentDto
{
    public Guid CircleId { get; set; }

    public int RoundNumber { get; set; }

    public string MemberId { get; set; } = string.Empty;
}