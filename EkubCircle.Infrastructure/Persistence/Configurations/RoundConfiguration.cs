using EkubCircle.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EkubCircle.Infrastructure.Persistence.Configurations;

public class RoundConfiguration
    : IEntityTypeConfiguration<Round>
{
    public void Configure(
        EntityTypeBuilder<Round> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.RoundNumber)
            .IsRequired();

        builder.Property(r => r.ReceiverId)
            .IsRequired();

        builder.Property(r => r.ContributionAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(r => r.PotAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(r => r.Status)
            .IsRequired();

        builder.Property(r => r.PaidOutAt);

        builder.HasIndex(r => new
        {
            r.CircleId,
            r.RoundNumber
        })
        .IsUnique();

        builder.HasOne(r => r.Circle)
            .WithMany(c => c.Rounds)
            .HasForeignKey(r => r.CircleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Receiver)
            .WithMany()
            .HasForeignKey(r => r.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.Payments)
            .WithOne(p => p.Round)
            .HasForeignKey(p => p.RoundId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}