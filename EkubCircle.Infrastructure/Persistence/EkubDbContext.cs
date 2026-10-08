using EkubCircle.Application.Interfaces;
using EkubCircle.Domain.Entities;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EkubCircle.Infrastructure.Persistence;

public class EkubDbContext :
    IdentityDbContext<ApplicationUser>,
    IEkubDbContext
{
    public EkubDbContext(
        DbContextOptions<EkubDbContext> options)
        : base(options)
    {
    }

    public DbSet<Circle> Circles => Set<Circle>();

    public DbSet<CircleMember> CircleMembers => Set<CircleMember>();

    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<Round> Rounds => Set<Round>();

    protected override void OnModelCreating(
        ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Circle>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.ContributionAmount)
                .HasPrecision(18, 2);

            entity.Property(x => x.MeetingLabel)
                .HasMaxLength(200)
                .IsRequired();

            entity.HasOne(x => x.Organizer)
                .WithMany()
                .HasForeignKey(x => x.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<CircleMember>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => new
            {
                x.CircleId,
                x.UserId
            })
            .IsUnique();

            entity.HasIndex(x => new
            {
                x.CircleId,
                x.Position
            })
            .IsUnique();

            entity.HasOne(x => x.Circle)
                .WithMany(x => x.Members)
                .HasForeignKey(x => x.CircleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Round>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => new
            {
                x.CircleId,
                x.RoundNumber
            })
            .IsUnique();

            entity.Property(x => x.ContributionAmount)
                .HasPrecision(18, 2);

            entity.Property(x => x.PotAmount)
                .HasPrecision(18, 2);

            entity.HasOne(x => x.Circle)
                .WithMany(x => x.Rounds)
                .HasForeignKey(x => x.CircleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Receiver)
                .WithMany()
                .HasForeignKey(x => x.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Payment>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => new
            {
                x.RoundId,
                x.MemberId
            })
            .IsUnique();

            entity.Property(x => x.Amount)
                .HasPrecision(18, 2);

            entity.HasOne(x => x.Round)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.RoundId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Member)
                .WithMany()
                .HasForeignKey(x => x.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}