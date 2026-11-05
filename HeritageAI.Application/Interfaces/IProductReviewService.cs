using HeritageAI.Application.Dto.Review;

namespace HeritageAI.Application.Interfaces
{
    public interface IProductReviewService
    {
        Task<ProductReviewDto> CreateAsync(int buyerId, CreateReviewDto dto);
        Task<ProductReviewsResponseDto> GetForProductAsync(int productId);
        Task<List<MyReviewDto>> GetMineAsync(int buyerId);
    }
}