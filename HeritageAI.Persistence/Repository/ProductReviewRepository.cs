using HeritageAI.Application.Interfaces;
using HeritageAI.Domain.Entities;
using HeritageAI.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HeritageAI.Persistence.Repository
{
    public class ProductReviewRepository : IProductReviewRepository
    {
        private readonly AppDbContext _context;
        public ProductReviewRepository(AppDbContext context) => _context = context;

        public async Task AddAsync(ProductReview review) =>
            await _context.ProductReviews.AddAsync(review);

        public async Task<ProductReview?> GetByIdAsync(int id) =>
            await _context.ProductReviews.Include(r => r.Buyer)
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<List<ProductReview>> GetByProductIdAsync(int productId) =>
            await _context.ProductReviews.Include(r => r.Buyer)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt).ToListAsync();

        public async Task<List<ProductReview>> GetByBuyerIdAsync(int buyerId) =>
            await _context.ProductReviews.Where(r => r.BuyerId == buyerId).ToListAsync();

        public async Task<bool> ExistsAsync(int productId, int buyerId) =>
            await _context.ProductReviews
                .AnyAsync(r => r.ProductId == productId && r.BuyerId == buyerId);

        public async Task<bool> HasDeliveredPurchaseAsync(int buyerId, int productId) =>
            await _context.OrderItems.AnyAsync(oi =>
                oi.ProductId == productId &&
                oi.Order.BuyerId == buyerId &&
                oi.Order.Status == OrderStatus.Delivered);

        public async Task<Dictionary<int, (double Average, int Count)>> GetStatsAsync(IEnumerable<int> productIds)
        {
            var ids = productIds.Distinct().ToList();
            var rows = await _context.ProductReviews
                .Where(r => ids.Contains(r.ProductId))
                .GroupBy(r => r.ProductId)
                .Select(g => new { ProductId = g.Key, Average = g.Average(r => (double)r.Rating), Count = g.Count() })
                .ToListAsync();
            return rows.ToDictionary(x => x.ProductId, x => (x.Average, x.Count));
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}