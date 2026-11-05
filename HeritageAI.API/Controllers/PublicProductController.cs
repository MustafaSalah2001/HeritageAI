using HeritageAI.Application.Dto.Product;
using HeritageAI.Application.Interfaces;
using HeritageAI.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace HeritageAI.API.Controllers
{
    [ApiController]
    [Route("api/public/products")]
    public class PublicProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductReviewRepository _reviewRepository;

        public PublicProductController(IProductRepository productRepository,
            IProductReviewRepository reviewRepository)
        {
            _productRepository = productRepository;
            _reviewRepository = reviewRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product is null
                || product.Status != ProductStatus.Published
                || !product.Craftsman.User.IsActive)
                return NotFound(new { message = "Product not found." });

            var stats = await _reviewRepository.GetStatsAsync(new[] { product.Id });
            stats.TryGetValue(product.Id, out var s);

            var result = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryName = product.Category.Name,
                EmbroideryStyle = product.EmbroideryStyle.ToString(),
                Colors = product.Colors,
                IsCustomizable = product.IsCustomizable,
                EstimatedProductionDays = product.EstimatedProductionDays,
                Status = product.Status.ToString(),
                CraftsmanName = product.Craftsman.User.FullName,
                CraftsmanId = product.CraftsmanId,
                CreatedAt = product.CreatedAt,
                AverageRating = Math.Round(s.Average, 1),
                ReviewCount = s.Count,
                Images = product.Images.Select(i => new ProductImageDto
                {
                    Id = i.Id,
                    Url = i.Url,
                    IsMain = i.IsMain
                }).ToList()
            };

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productRepository.GetPublishedAsync();
            var stats = await _reviewRepository.GetStatsAsync(products.Select(p => p.Id));

            var result = products.Select(p =>
            {
                stats.TryGetValue(p.Id, out var s);
                return new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    CategoryName = p.Category.Name,
                    EmbroideryStyle = p.EmbroideryStyle.ToString(),
                    Colors = p.Colors,
                    IsCustomizable = p.IsCustomizable,
                    EstimatedProductionDays = p.EstimatedProductionDays,
                    Status = p.Status.ToString(),
                    CraftsmanName = p.Craftsman.User.FullName,
                    CraftsmanId = p.CraftsmanId,
                    CreatedAt = p.CreatedAt,
                    AverageRating = Math.Round(s.Average, 1),
                    ReviewCount = s.Count,
                    Images = p.Images.Select(i => new ProductImageDto
                    {
                        Id = i.Id,
                        Url = i.Url,
                        IsMain = i.IsMain
                    }).ToList()
                };
            }).ToList();

            return Ok(result);
        }
    }
}