using EkubCircle.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EkubCircle.Infrastructure.Persistence.Configurations;

public class CircleMemberConfiguration
    : IEntityTypeConfiguration<CircleMember>
{
    public void Configure(
        EntityTypeBuilder<CircleMember> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.UserId)
            .IsRequired();

        builder.Property(m => m.Position)
            .IsRequired();

        builder.Property(m => m.HasReceived)
            .IsRequired();

        builder.HasIndex(m => m.UserId);

        builder.HasIndex(m => new
        {
            m.CircleId,
            m.UserId
        })
        .IsUnique();

        builder.HasIndex(m => new
        {
            m.CircleId,
            m.Position
        })
        .IsUnique();

        builder.HasOne(m => m.Circle)
            .WithMany(c => c.Members)
            .HasForeignKey(m => m.CircleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}