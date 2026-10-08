using System.Security.Claims;

using EkubCircle.Application.DTOs.Circle;
using EkubCircle.Application.DTOs.Member;
using EkubCircle.Application.Interfaces;

using EkubCircle.Domain.Entities;
using EkubCircle.Domain.Enums;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EkubCircle.Application.DTOs.Round;

namespace EkubCircle.Infrastructure.Services;

public class CircleService : ICircleService
{
    private readonly IEkubDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CircleService(
        IEkubDbContext db,
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    // =========================================================
    // CREATE CIRCLE
    // =========================================================

    public async Task<CircleResponseDto> CreateCircleAsync(
        CircleDetailDto request)
    {
        var organizerId = _httpContextAccessor.HttpContext?
            .User
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value;

        if (string.IsNullOrEmpty(organizerId))
        {
            throw new UnauthorizedAccessException(
                "User is not authenticated.");
        }

        var organizer =
            await _userManager.FindByIdAsync(organizerId);

        if (organizer == null)
        {
            throw new KeyNotFoundException(
                "Organizer not found.");
        }

        var isOrganizer =
            await _userManager.IsInRoleAsync(
                organizer,
                "Organizer");

        if (!isOrganizer)
        {
            throw new UnauthorizedAccessException(
                "Only an Organizer can create a circle.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException(
                "Circle name is required.");
        }

        if (request.ContributionAmount <= 0)
        {
            throw new ArgumentException(
                "Contribution amount must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(request.MeetingLabel))
        {
            throw new ArgumentException(
                "Meeting label is required.");
        }

        var circle = new Circle
        {
            Id = Guid.NewGuid(),

            Name = request.Name.Trim(),

            ContributionAmount =
                request.ContributionAmount,

            MeetingLabel =
                request.MeetingLabel.Trim(),

            OrganizerId = organizerId,

            Status = CircleStatus.Forming,

            MemberCount = 1,

            CurrentRoundNumber = 1,

            CurrentReceiverPosition = 1,

            CreatedAt = DateTime.UtcNow
        };

        _db.Circles.Add(circle);

        // Organizer is also a CircleMember.
        var organizerMember = new CircleMember
        {
            Id = Guid.NewGuid(),

            CircleId = circle.Id,

            UserId = organizerId,

            Position = 1,

            HasReceived = false
        };

        _db.CircleMembers.Add(organizerMember);

        await _db.SaveChangesAsync();

        return new CircleResponseDto
        {
            Id = circle.Id,

            Name = circle.Name,

            ContributionAmount =
                circle.ContributionAmount,

            MeetingLabel =
                circle.MeetingLabel,

            OrganizerId =
                circle.OrganizerId
        };
    }

    // =========================================================
    // ADD MEMBER
    // =========================================================

    public async Task AddMemberAsync(
        Guid circleId,
        AddMemberDto request,
        string organizerId)
    {
        var circle = await _db.Circles
            .Include(x => x.Members)
            .FirstOrDefaultAsync(x =>
                x.Id == circleId);

        if (circle == null)
        {
            throw new KeyNotFoundException(
                "Circle not found.");
        }

        if (circle.OrganizerId != organizerId)
        {
            throw new UnauthorizedAccessException(
                "Only the organizer can add members.");
        }

        // Members can only be added while forming.
        if (circle.Status != CircleStatus.Forming)
        {
            throw new InvalidOperationException(
                "Members cannot be added after the circle has started.");
        }

        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.TemporaryPassword))
        {
            throw new ArgumentException(
                "Name, email and temporary password are required.");
        }

        var normalizedEmail =
            request.Email.Trim().ToLowerInvariant();

        var existingUser =
            await _userManager.FindByEmailAsync(
                normalizedEmail);

        if (existingUser != null)
        {
            throw new InvalidOperationException(
                "A user with this email already exists.");
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),

            UserName = normalizedEmail,

            Email = normalizedEmail,

            FullName = request.Name.Trim(),

            MustChangePassword = true
        };

        var result =
            await _userManager.CreateAsync(
                user,
                request.TemporaryPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                "; ",
                result.Errors.Select(
                    x => x.Description));

            throw new InvalidOperationException(errors);
        }

        var roleResult =
            await _userManager.AddToRoleAsync(
                user,
                "Member");

        if (!roleResult.Succeeded)
        {
            var errors = string.Join(
                "; ",
                roleResult.Errors.Select(
                    x => x.Description));

            throw new InvalidOperationException(errors);
        }

        var nextPosition =
            (circle.Members.Max(
                x => (int?)x.Position) ?? 0) + 1;

        var member = new CircleMember
        {
            Id = Guid.NewGuid(),

            CircleId = circle.Id,

            UserId = user.Id,

            Position = nextPosition,

            HasReceived = false
        };

        _db.CircleMembers.Add(member);

        circle.MemberCount = nextPosition;

        await _db.SaveChangesAsync();
    }

    // =========================================================
    // GET MEMBERS
    // =========================================================

    public async Task<IEnumerable<MemberResponseDto>>
        GetMembersAsync(Guid circleId)
    {
        var members = await _db.CircleMembers
            .AsNoTracking()
            .Where(x => x.CircleId == circleId)
            .Include(x => x.User)
            .OrderBy(x => x.Position)
            .ToListAsync();

        return members
            .Select(x => new MemberResponseDto
            {
                UserId = x.UserId,

                Name =
                    x.User?.FullName ??
                    string.Empty,

                Email =
                    x.User?.Email ??
                    string.Empty
            })
            .ToList();
    }

    // =========================================================
    // GET ORGANIZER'S CIRCLE
    // =========================================================

    public async Task<CircleDetailDto?>
        GetMyCircleDetailsAsync(
            string organizerId)
    {
        var circle = await _db.Circles
            .AsNoTracking()

            .Include(x => x.Members)
                .ThenInclude(x => x.User)

            .Include(x => x.Rounds)
                .ThenInclude(x => x.Receiver)

            .Include(x => x.Rounds)
                .ThenInclude(x => x.Payments)

            .FirstOrDefaultAsync(
                x => x.OrganizerId == organizerId);

        if (circle == null)
        {
            return null;
        }

        var currentRound = circle.Rounds
            .FirstOrDefault(
                x => x.RoundNumber ==
                     circle.CurrentRoundNumber);

        var members = circle.Members
            .OrderBy(x => x.Position)
            .Select(member => new MemberPaymentDto
            {
                MemberId = member.UserId,

                FullName =
                    member.User?.FullName ??
                    string.Empty,

                HasPaid =
                    currentRound?.Payments.Any(
                        payment =>
                            payment.MemberId ==
                            member.UserId) ?? false,

                HasReceived =
                    member.HasReceived
            })
            .ToList();

        var rounds = circle.Rounds
            .OrderBy(x => x.RoundNumber)
            .Select(round => new RoundInfoDto
            {
                RoundNumber =
                    round.RoundNumber,

                Status =
                    round.Status.ToString(),

                DesignatedReceiver =
                    round.Receiver?.FullName ??
                    string.Empty
            })
            .ToList();

        return new CircleDetailDto
        {

            Name =
                circle.Name,

            ContributionAmount =
                circle.ContributionAmount,

            MeetingLabel =
                circle.MeetingLabel,

        };
    }
    // =========================================================
    // START CIRCLE
    // =========================================================
public async Task StartCircleAsync(
    Guid circleId,
    string organizerId)
{
    var circle = await _db.Circles
        .Include(x => x.Members)
        .Include(x => x.Rounds)
        .FirstOrDefaultAsync(x =>
            x.Id == circleId &&
            x.OrganizerId == organizerId);

    if (circle == null)
    {
        throw new KeyNotFoundException(
            "Circle not found.");
    }

    if (circle.Status != CircleStatus.Forming)
    {
        throw new InvalidOperationException(
            "This circle has already been started.");
    }

    if (circle.Members.Count < 2)
    {
        throw new InvalidOperationException(
            "A circle must have at least two members before it can start.");
    }

    var members = circle.Members
        .OrderBy(x => x.Position)
        .ToList();

    // Validate positions
    for (var i = 0; i < members.Count; i++)
    {
        if (members[i].Position != i + 1)
        {
            throw new InvalidOperationException(
                "Member payout positions must start at 1 and be consecutive.");
        }
    }

    // Make sure rounds don't already exist.
    if (circle.Rounds.Any())
    {
        throw new InvalidOperationException(
            "Rounds already exist for this circle.");
    }

    // Create rounds separately.
    var rounds = new List<Round>();

    for (var i = 0; i < members.Count; i++)
    {
        var member = members[i];

        rounds.Add(new Round
        {
            Id = Guid.NewGuid(),

            CircleId = circle.Id,

            RoundNumber = i + 1,

            ReceiverId = member.UserId,

            ContributionAmount =
                circle.ContributionAmount,

            // Nobody has paid yet.
            PotAmount = 0,

            Status = RoundStatus.Open
        });
    }

    // Add rounds directly to DbContext.
    _db.Rounds.AddRange(rounds);

    // Update circle.
    circle.Status = CircleStatus.Active;

    circle.CurrentRoundNumber = 1;

    circle.CurrentReceiverPosition = 1;

    circle.MemberCount = members.Count;

    await _db.SaveChangesAsync();
}

    // =========================================================
    // GET SINGLE CIRCLE
    // =========================================================



   public async Task<CircleDetailDto?> GetCircleAsync(Guid circleId)
{
    var circle = await _db.Circles
        .AsNoTracking()
        .Include(x => x.Members)
            .ThenInclude(x => x.User)
        .Include(x => x.Rounds)
            .ThenInclude(x => x.Receiver)
        .Include(x => x.Rounds)
            .ThenInclude(x => x.Payments)
        .FirstOrDefaultAsync(x => x.Id == circleId);

    if (circle == null)
    {
        return null;
    }

    var currentRound = circle.Rounds
        .FirstOrDefault(x =>
            x.RoundNumber == circle.CurrentRoundNumber);

    var members = circle.Members
        .OrderBy(x => x.Position)
        .Select(member => new MemberPaymentDto
        {
            MemberId = member.UserId,
            FullName = member.User?.FullName ?? string.Empty,

            HasPaid = currentRound?.Payments
                .Any(payment =>
                    payment.MemberId == member.UserId) ?? false,

            HasReceived = member.HasReceived
        })
        .ToList();

    var rounds = circle.Rounds
        .OrderBy(x => x.RoundNumber)
        .Select(round => new RoundInfoDto
        {
            RoundNumber = round.RoundNumber,
            Status = round.Status.ToString(),
            DesignatedReceiver =
                round.Receiver?.FullName ?? string.Empty
        })
        .ToList();

    return new CircleDetailDto
    {
        Name = circle.Name,
        ContributionAmount = circle.ContributionAmount,
        MeetingLabel = circle.MeetingLabel
    };
}

    public async Task<IEnumerable<CircleResponseDto>> GetAllCirclesAsync()
{
    var circles = await _db.Circles
        .AsNoTracking()
        .OrderByDescending(x => x.CreatedAt)
        .ToListAsync();

    return circles.Select(circle => new CircleResponseDto
    {
        Id = circle.Id,
        Name = circle.Name,
        ContributionAmount = circle.ContributionAmount,
        MeetingLabel = circle.MeetingLabel,
        OrganizerId = circle.OrganizerId
    }).ToList();
}
public async Task TogglePaymentAsync(
    Guid circleId,
    int roundNumber,
    string memberId,
    string organizerId)
{
    var circle = await _db.Circles
        .Include(x => x.Members)
        .Include(x => x.Rounds)
            .ThenInclude(x => x.Payments)
        .FirstOrDefaultAsync(x =>
            x.Id == circleId &&
            x.OrganizerId == organizerId);

    if (circle == null)
    {
        throw new KeyNotFoundException(
            "Circle not found.");
    }

    if (circle.Status != CircleStatus.Active)
    {
        throw new InvalidOperationException(
            "Payments can only be recorded for an active circle.");
    }

    if (roundNumber != circle.CurrentRoundNumber)
    {
        throw new InvalidOperationException(
            "Payments can only be recorded for the current round.");
    }

    var round = circle.Rounds
        .FirstOrDefault(x =>
            x.RoundNumber == roundNumber);

    if (round == null)
    {
        throw new KeyNotFoundException(
            "Round not found.");
    }

    if (round.Status != RoundStatus.Open)
    {
        throw new InvalidOperationException(
            "Payments cannot be changed after the round has been paid out.");
    }

    var member = circle.Members
        .FirstOrDefault(x =>
            x.UserId == memberId);

    if (member == null)
    {
        throw new KeyNotFoundException(
            "Member does not belong to this circle.");
    }

    var existingPayment = round.Payments
        .FirstOrDefault(x =>
            x.MemberId == memberId);

    // Do not allow paying the same member twice.
    if (existingPayment != null)
    {
        throw new InvalidOperationException(
            "This member has already been marked as paid for this round.");
    }

    var payment = new Payment
    {
        Id = Guid.NewGuid(),

        RoundId = round.Id,

        MemberId = memberId,

        Amount = circle.ContributionAmount,

        PaidAt = DateTime.UtcNow
    };

    _db.Payments.Add(payment);

    // Pot = number of paid members × contribution.
    var paidCount = round.Payments.Count + 1;

    round.PotAmount =
        paidCount * circle.ContributionAmount;

    await _db.SaveChangesAsync();
}
}