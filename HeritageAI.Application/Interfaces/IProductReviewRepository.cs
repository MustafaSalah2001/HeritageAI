using HeritageAI.Domain.Entities;

namespace HeritageAI.Application.Interfaces
{
    public interface IProductReviewRepository
    {
        Task AddAsync(ProductReview review);
        Task<ProductReview?> GetByIdAsync(int id);
        Task<List<ProductReview>> GetByProductIdAsync(int productId);
        Task<List<ProductReview>> GetByBuyerIdAsync(int buyerId);
        Task<bool> ExistsAsync(int productId, int buyerId);
        Task<bool> HasDeliveredPurchaseAsync(int buyerId, int productId);
        Task<Dictionary<int, (double Average, int Count)>> GetStatsAsync(IEnumerable<int> productIds);
        Task SaveChangesAsync();
    }
}