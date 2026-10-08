using EkubCircle.Application.DTOs.Round;
using EkubCircle.Application.Interfaces;
using EkubCircle.Domain.Entities;
using EkubCircle.Domain.Enums;
using Microsoft.EntityFrameworkCore;
namespace EkubCircle.Infrastructure.Services;

public class LotteryService : ILotteryService
{
    private readonly IEkubDbContext _db;

    public LotteryService(IEkubDbContext db)
    {
        _db = db;
    }

    public async Task<RoundResponseDto> ConductLotteryDrawAsync(
        Guid roundId)
    {
        return await GetCurrentRoundAsync(roundId);
    }

    public async Task<RoundResponseDto> GetCurrentRoundAsync(
        Guid circleId)
    {
        var circle = await _db.Circles
            .Include(x => x.Members)
            .FirstOrDefaultAsync(x => x.Id == circleId);

        if (circle == null)
            throw new KeyNotFoundException("Circle not found.");

        if (circle.Status == CircleStatus.Completed)
            throw new InvalidOperationException(
                "Circle is already completed.");

        if (circle.Members.Count == 0)
            throw new InvalidOperationException(
                "Circle has no members.");

        var existingRound = await _db.Rounds
            .Include(x => x.Payments)
            .FirstOrDefaultAsync(x =>
                x.CircleId == circleId &&
                x.RoundNumber == circle.CurrentRoundNumber);

        if (existingRound != null)
        {
            return MapRound(
                existingRound,
                circle.MemberCount);
        }

        var receiver = circle.Members
            .OrderBy(x => x.Position)
            .FirstOrDefault(x =>
                x.Position == circle.CurrentReceiverPosition);

        if (receiver == null)
            throw new InvalidOperationException(
                "Receiver could not be determined.");

        var round = new Round
        {
            Id = Guid.NewGuid(),
            CircleId = circleId,
            RoundNumber = circle.CurrentRoundNumber,
            ReceiverId = receiver.UserId,
            ContributionAmount = circle.ContributionAmount,
            PotAmount =
                circle.ContributionAmount *
                circle.MemberCount,
            Status = RoundStatus.Open
        };

        _db.Rounds.Add(round);

        await _db.SaveChangesAsync();

        return MapRound(
            round,
            circle.MemberCount);
    }

    public async Task<RoundResponseDto> CompleteRoundAsync(
        Guid circleId)
    {
        var circle = await _db.Circles
            .Include(x => x.Members)
            .FirstOrDefaultAsync(x => x.Id == circleId);

        if (circle == null)
            throw new KeyNotFoundException(
                "Circle not found.");

        if (circle.Status == CircleStatus.Completed)
            throw new InvalidOperationException(
                "Circle is already completed.");

        if (circle.Members.Count == 0)
            throw new InvalidOperationException(
                "Circle has no members.");

        var round = await _db.Rounds
            .Include(x => x.Payments)
            .FirstOrDefaultAsync(x =>
                x.CircleId == circleId &&
                x.RoundNumber == circle.CurrentRoundNumber);

        if (round == null)
            throw new InvalidOperationException(
                "The current round has not been created.");

        if (round.Status == RoundStatus.PaidOut)
            throw new InvalidOperationException(
                "This round has already been paid out.");

        var paidCount = round.Payments.Count;

        if (paidCount < circle.MemberCount)
        {
            throw new InvalidOperationException(
                $"All members must pay before payout. " +
                $"Paid: {paidCount}/{circle.MemberCount}.");
        }

        if (paidCount > circle.MemberCount)
        {
            throw new InvalidOperationException(
                "Payment count is invalid for this round.");
        }

        var receiver = circle.Members
            .FirstOrDefault(x =>
                x.UserId == round.ReceiverId);

        if (receiver == null)
            throw new InvalidOperationException(
                "Round receiver could not be found.");

        if (receiver.HasReceived)
            throw new InvalidOperationException(
                "This member has already received a payout.");

        // Mark this round as paid out.
        round.Status = RoundStatus.PaidOut;
        round.PaidOutAt = DateTime.UtcNow;

        // Mark receiver as having received their payout.
        receiver.HasReceived = true;

        var allMembersReceived = circle.Members
            .All(x => x.HasReceived);

        if (allMembersReceived)
        {
            circle.Status = CircleStatus.Completed;
        }
        else
        {
            circle.Status = CircleStatus.Active;

            var nextReceiver = circle.Members
                .Where(x => !x.HasReceived)
                .OrderBy(x => x.Position)
                .FirstOrDefault();

            if (nextReceiver == null)
            {
                circle.Status = CircleStatus.Completed;
            }
            else
            {
                circle.CurrentReceiverPosition =
                    nextReceiver.Position;

                circle.CurrentRoundNumber++;
            }
        }

        await _db.SaveChangesAsync();

        return MapRound(
            round,
            circle.MemberCount);
    }

    public async Task<IEnumerable<RoundResponseDto>> GetCircleRoundsAsync(
        Guid circleId)
    {
        var circle = await _db.Circles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == circleId);

        if (circle == null)
            throw new KeyNotFoundException(
                "Circle not found.");

        var rounds = await _db.Rounds
            .AsNoTracking()
            .Include(x => x.Payments)
            .Where(x => x.CircleId == circleId)
            .OrderBy(x => x.RoundNumber)
            .ToListAsync();

        return rounds.Select(x =>
            MapRound(
                x,
                circle.MemberCount));
    }

    private static RoundResponseDto MapRound(
        Round round,
        int memberCount)
    {
        return new RoundResponseDto
        {
            Id = round.Id,
            CircleId = round.CircleId,
            RoundNumber = round.RoundNumber,
            ReceiverId = round.ReceiverId,
            ContributionAmount =
                round.ContributionAmount,
            PotAmount = round.PotAmount,
            PaidMemberCount =
                round.Payments?.Count ?? 0,
            TotalMemberCount = memberCount,
            IsCompleted =
                round.Status == RoundStatus.PaidOut
        };
    }
}