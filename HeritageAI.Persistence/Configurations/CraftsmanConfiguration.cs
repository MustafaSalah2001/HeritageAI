using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HeritageAI.Domain.Entities;

namespace HeritageAI.Persistence.Configurations;

public class CraftsmanConfiguration : IEntityTypeConfiguration<Craftsman>
{
    public void Configure(EntityTypeBuilder<Craftsman> builder)
    {
        // UserId هو الـ Primary Key هنا (مش Id منفصل)
        builder.HasKey(c => c.UserId);

        // العلاقة One-to-One مع User
        builder.HasOne(c => c.User)
            .WithOne(u => u.Craftsman)
            .HasForeignKey<Craftsman>(c => c.UserId);

        builder.Property(c => c.Bio)
            .HasMaxLength(1000);

        builder.Property(c => c.StoryText)
            .HasMaxLength(2000);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}