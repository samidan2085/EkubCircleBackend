using EkubCircle.Application.Interfaces;
using EkubCircle.Domain.Entities;
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
        var circle = await _db.Circles
            .Include(x => x.Members)
            .FirstOrDefaultAsync(x => x.Id == circleId);

        if (circle == null)
            throw new KeyNotFoundException(
                "Circle not found.");

        if (circle.Status == CircleStatus.Completed)
            throw new InvalidOperationException(
                "This circle is already completed.");

        var member = circle.Members
            .FirstOrDefault(x => x.UserId == memberId);

        if (member == null)
            throw new UnauthorizedAccessException(
                "User is not a member of this circle.");

        var round = await _db.Rounds
            .Include(x => x.Payments)
            .FirstOrDefaultAsync(x =>
                x.CircleId == circleId &&
                x.RoundNumber == circle.CurrentRoundNumber);

        if (round == null)
        {
            var receiver = circle.Members
                .OrderBy(x => x.Position)
                .FirstOrDefault(x =>
                    x.Position == circle.CurrentReceiverPosition);

            if (receiver == null)
                throw new InvalidOperationException(
                    "Receiver could not be determined.");

            round = new Round
            {
                Id = Guid.NewGuid(),
                CircleId = circleId,
                RoundNumber = circle.CurrentRoundNumber,
                ReceiverId = receiver.UserId,
                ContributionAmount = circle.ContributionAmount,
                PotAmount =
                    circle.ContributionAmount * circle.MemberCount,
                Status = RoundStatus.Open
            };

            _db.Rounds.Add(round);

            await _db.SaveChangesAsync();
        }

        var alreadyPaid = await _db.Payments.AnyAsync(x =>
            x.RoundId == round.Id &&
            x.MemberId == memberId);

        if (alreadyPaid)
            throw new InvalidOperationException(
                "Member has already paid for this round.");

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            RoundId = round.Id,
            MemberId = memberId,
            Amount = round.ContributionAmount,
            PaidAt = DateTime.UtcNow
        };

        _db.Payments.Add(payment);

        await _db.SaveChangesAsync();

        var paidCount = await _db.Payments.CountAsync(x =>
            x.RoundId == round.Id);

        var totalMembers = circle.MemberCount;

        return new
        {
            payment.Id,
            payment.Amount,
            payment.PaidAt,
            PaidMembers = paidCount,
            TotalMembers = totalMembers,
            AllPaid = paidCount == totalMembers
        };
    }
}