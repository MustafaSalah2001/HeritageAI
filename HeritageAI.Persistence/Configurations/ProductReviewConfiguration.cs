using HeritageAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeritageAI.Persistence.Configurations
{
    public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
    {
        public void Configure(EntityTypeBuilder<ProductReview> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Comment).HasMaxLength(500);

            // تقييم واحد لكل مشتري على كل منتج
            builder.HasIndex(r => new { r.ProductId, r.BuyerId }).IsUnique();

            builder.HasOne(r => r.Product).WithMany()
                .HasForeignKey(r => r.ProductId).OnDelete(DeleteBehavior.Cascade);

            // Restrict عشان SQL Server ما يشتكي من multiple cascade paths
            builder.HasOne(r => r.Buyer).WithMany()
                .HasForeignKey(r => r.BuyerId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}