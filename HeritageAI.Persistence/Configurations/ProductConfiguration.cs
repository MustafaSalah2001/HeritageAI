using HeritageAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HeritageAI.Domain.Enums;
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.Price)
            .HasPrecision(10, 2); // مهم للـ decimal: 10 أرقام كلها، 2 بعد الفاصلة

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.EmbroideryStyle)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasOne(p => p.Craftsman)
            .WithMany() // Craftsman.Products مش عاملينها Navigation دلوقتي، وده تمام
            .HasForeignKey(p => p.CraftsmanId)
            .OnDelete(DeleteBehavior.Restrict); // مايتمسحش المنتج تلقائي لو الحرفي اتمسح

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}