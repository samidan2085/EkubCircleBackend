namespace EkubCircle.Application.Interfaces;

public interface IPaymentService
{
    Task<object> RecordPaymentAsync(
        Guid circleId,
        string memberId);
}