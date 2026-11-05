using Microsoft.EntityFrameworkCore;
using HeritageAI.Domain.Entities;

namespace HeritageAI.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Craftsman> Craftsmen { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<ProductReview> ProductReviews { get; set; }
    public DbSet<CustomOrderRequest> CustomOrderRequests { get; set; }
    public DbSet<SiteContent> SiteContents { get; set; }
    public DbSet<CraftsmanSkill> CraftsmanSkills { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<HomepageImage> HomepageImages { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<OrderTransfer> OrderTransfers { get; set; }
    public DbSet<Testimonial> Testimonials { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}