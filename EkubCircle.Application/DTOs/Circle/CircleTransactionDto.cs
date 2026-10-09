namespace EkubCircle.Application.DTOs.Circle;

public class CircleTransactionDto
{
    public Guid PaymentId { get; set; }
    public string MemberId { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public int RoundNumber { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }
    public string Status { get; set; } = "Paid";
}