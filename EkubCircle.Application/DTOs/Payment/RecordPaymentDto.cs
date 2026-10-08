namespace EkubCircle.Application.DTOs.Payment;

public class RecordPaymentDto
{
    public Guid CircleId { get; set; }

    public string MemberId { get; set; } = string.Empty;
}