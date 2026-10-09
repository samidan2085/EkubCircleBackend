using EkubCircle.Application.Interfaces;
using EkubCircle.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EkubCircle.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly IEkubDbContext _db;

    public PaymentService(IEkubDbContext db)
    {
        _db = db;
    }

    public async Task<object> RecordPaymentAsync(
        Guid circleId,
        string memberId)
    {
        // 1. Find the circle and its current round.
        var circle = await _db.Circles
            .Include(c => c.Members)
            .Include(c => c.Rounds)
                .ThenInclude(r => r.Payments)
            .FirstOrDefaultAsync(c => c.Id == circleId);

        if (circle is null)
            throw new KeyNotFoundException("Circle not found.");

        // 2. Verify that the logged-in user belongs to this circle.
        var membership = circle.Members
            .FirstOrDefault(m => m.UserId == memberId);

        if (membership is null)
            throw new UnauthorizedAccessException(
                "You are not a member of this circle.");

        // 3. Verify that the circle is active.
        if (circle.Status != CircleStatus.Active)
            throw new InvalidOperationException(
                "This circle is not active.");

        // 4. Find the current round.
        var round = circle.Rounds
            .FirstOrDefault(r =>
                r.RoundNumber == circle.CurrentRoundNumber);

        if (round is null)
            throw new InvalidOperationException(
                "The current round was not found.");

        if (round.Status != RoundStatus.Open)
            throw new InvalidOperationException(
                "The current round is not open for payments.");

        // 5. Prevent duplicate payments.
        var alreadyPaid = round.Payments
            .Any(p => p.MemberId == memberId);

        if (alreadyPaid)
            throw new InvalidOperationException(
                "You have already paid for this round.");

        // 6. Record the payment.
        var payment = new EkubCircle.Domain.Entities.Payment
        {
            Id = Guid.NewGuid(),
            MemberId = memberId,
            Amount = circle.ContributionAmount,
            PaidAt = DateTime.UtcNow,
            RoundId = round.Id
        };

        _db.Payments.Add(payment);

        // 7. Update the pot.
        round.PotAmount += circle.ContributionAmount;

        await _db.SaveChangesAsync();

        return new
        {
            Message = "Payment recorded successfully.",
            CircleId = circle.Id,
            RoundNumber = round.RoundNumber,
            MemberId = memberId,
            Amount = payment.Amount,
            PotAmount = round.PotAmount,
            PaidAt = payment.PaidAt
        };
    }
}