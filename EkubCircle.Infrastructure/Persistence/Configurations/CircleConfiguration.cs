using EkubCircle.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EkubCircle.Infrastructure.Persistence.Configurations;

public class CircleConfiguration : IEntityTypeConfiguration<Circle>
{
    public void Configure(EntityTypeBuilder<Circle> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.ContributionAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(c => c.MemberCount)
            .IsRequired();

        builder.Property(c => c.MeetingLabel)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.OrganizerId)
            .IsRequired();

        builder.Property(c => c.Status)
            .IsRequired();

        builder.Property(c => c.CurrentRoundNumber)
            .IsRequired();

        builder.Property(c => c.CurrentReceiverPosition)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.HasIndex(c => c.OrganizerId);

        builder.HasMany(c => c.Members)
            .WithOne(m => m.Circle)
            .HasForeignKey(m => m.CircleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Rounds)
            .WithOne(r => r.Circle)
            .HasForeignKey(r => r.CircleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Organizer)
            .WithMany()
            .HasForeignKey(c => c.OrganizerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}