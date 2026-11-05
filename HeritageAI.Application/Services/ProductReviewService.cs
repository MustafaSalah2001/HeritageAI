using HeritageAI.Application.Dto.Review;
using HeritageAI.Application.Interfaces;
using HeritageAI.Domain.Entities;

namespace HeritageAI.Application.Services
{
    public class ProductReviewService : IProductReviewService
    {
        private readonly IProductReviewRepository _reviews;
        private readonly IProductRepository _products;
        private readonly INotificationRepository _notifications;

        public ProductReviewService(IProductReviewRepository reviews,
            IProductRepository products, INotificationRepository notifications)
        {
            _reviews = reviews;
            _products = products;
            _notifications = notifications;
        }

        public async Task<ProductReviewDto> CreateAsync(int buyerId, CreateReviewDto dto)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
                throw new InvalidOperationException("Rating must be between 1 and 5.");

            var comment = string.IsNullOrWhiteSpace(dto.Comment) ? null : dto.Comment.Trim();
            if (comment is not null && comment.Length > 500)
                throw new InvalidOperationException("Review text is too long.");

            var product = await _products.GetByIdAsync(dto.ProductId);
            if (product is null)
                throw new InvalidOperationException("Product not found.");

            if (!await _reviews.HasDeliveredPurchaseAsync(buyerId, dto.ProductId))
                throw new InvalidOperationException("You can only review products from delivered orders.");

            if (await _reviews.ExistsAsync(dto.ProductId, buyerId))
                throw new InvalidOperationException("You have already reviewed this product.");

            var review = new ProductReview
            {
                ProductId = dto.ProductId,
                BuyerId = buyerId,
                Rating = dto.Rating,
                Comment = comment,
                CreatedAt = DateTime.UtcNow
            };
            await _reviews.AddAsync(review);

            // الإشعار بينحفظ مع التقييم بنفس الـ SaveChanges
            _notifications.Add(new Notification
            {
                UserId = product.CraftsmanId,
                Title = "تقييم جديد على منتجك",
                Message = $"قيّم أحد المشترين \"{product.Name}\" بـ {dto.Rating} من 5.",
                Link = "/dashboard",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });

            await _reviews.SaveChangesAsync();

            var created = await _reviews.GetByIdAsync(review.Id);
            return MapToDto(created!);
        }

        public async Task<ProductReviewsResponseDto> GetForProductAsync(int productId)
        {
            var items = await _reviews.GetByProductIdAsync(productId);
            return new ProductReviewsResponseDto
            {
                ReviewCount = items.Count,
                AverageRating = items.Count == 0 ? 0 : Math.Round(items.Average(r => r.Rating), 1),
                Reviews = items.Select(MapToDto).ToList()
            };
        }

        public async Task<List<MyReviewDto>> GetMineAsync(int buyerId)
        {
            var items = await _reviews.GetByBuyerIdAsync(buyerId);
            return items.Select(r => new MyReviewDto
            {
                ProductId = r.ProductId,
                Rating = r.Rating,
                Comment = r.Comment
            }).ToList();
        }

        // "مصطفى ابراهيم" بيظهر "مصطفى ا."
        private static string MaskName(string fullName)
        {
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return "مشتري";
            if (parts.Length == 1) return parts[0];
            return $"{parts[0]} {parts[^1][0]}.";
        }

        private static ProductReviewDto MapToDto(ProductReview r) => new()
        {
            Id = r.Id,
            ProductId = r.ProductId,
            BuyerName = MaskName(r.Buyer.FullName),
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        };
    }
}