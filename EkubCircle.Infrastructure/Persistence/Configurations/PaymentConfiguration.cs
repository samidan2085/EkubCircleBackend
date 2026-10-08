using EkubCircle.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EkubCircle.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration
    : IEntityTypeConfiguration<Payment>
{
    public void Configure(
        EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.MemberId)
            .IsRequired();

        builder.Property(p => p.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.PaidAt)
            .IsRequired();

        builder.HasIndex(p => new
        {
            p.RoundId,
            p.MemberId
        })
        .IsUnique();

        builder.HasOne(p => p.Round)
            .WithMany(r => r.Payments)
            .HasForeignKey(p => p.RoundId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Member)
            .WithMany()
            .HasForeignKey(p => p.MemberId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}