using HeritageAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CustomOrderRequestConfiguration : IEntityTypeConfiguration<CustomOrderRequest>
{
    public void Configure(EntityTypeBuilder<CustomOrderRequest> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(c => c.ProposedPrice)
            .HasPrecision(10, 2);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasOne(c => c.Buyer)
            .WithMany()
            .HasForeignKey(c => c.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Craftsman)
            .WithMany()
            .HasForeignKey(c => c.CraftsmanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}