using EkubCircle.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EkubCircle.Application.Interfaces;

public interface IEkubDbContext
{
    DbSet<Circle> Circles { get; }

    DbSet<CircleMember> CircleMembers { get; }

    DbSet<Payment> Payments { get; }

    DbSet<Round> Rounds { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}