using HeritageAI.Application.Dto.Product;
using HeritageAI.Application.Interfaces;
using HeritageAI.Domain.Entities;
using HeritageAI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICraftsmanRepository _craftsmanRepository;
        private readonly INotificationRepository _notifications;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            ICraftsmanRepository craftsmanRepository,
            INotificationRepository notifications)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _craftsmanRepository = craftsmanRepository;
            _notifications = notifications;
        }

        public async Task<List<ProductDto>> GetMyProductsAsync(int craftsmanId)
        {
            var products = await _productRepository.GetByCraftsmanIdAsync(craftsmanId);
            return products.Select(MapToDto).ToList();
        }

        public async Task<ProductDto> CreateAsync(int craftsmanId, CreateProductDto dto)
        {
            var craftsman = await _craftsmanRepository.GetByUserIdAsync(craftsmanId);

            if (craftsman is null)
                throw new InvalidOperationException("Craftsman profile not found.");
            if (craftsman.Status == CraftsmanStatus.Rejected)
                throw new InvalidOperationException("Your account was rejected, so you cannot add products.");
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category is null)
                throw new InvalidOperationException("Category not found.");

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                CraftsmanId = craftsmanId,
                EmbroideryStyle = dto.EmbroideryStyle,
                Colors = dto.Colors,
                IsCustomizable = dto.IsCustomizable,
                EstimatedProductionDays = dto.EstimatedProductionDays,
                Status = craftsman.Status == CraftsmanStatus.Approved
                    ? ProductStatus.PendingReview
                    : ProductStatus.Draft,
                CreatedAt = DateTime.UtcNow
            };

            await _productRepository.AddAsync(product);
            if (product.Status == ProductStatus.PendingReview)
            {
                await _notifications.AddForAdminsAsync(
                    "منتج جديد بانتظار المراجعة",
                    $"أضاف حرفي المنتج \"{product.Name}\" وبانتظار نشره.",
                    "/admin/products");
            }
            await _productRepository.SaveChangesAsync();

            var created = await _productRepository.GetByIdAsync(product.Id);
            return MapToDto(created!);
        }
        public async Task<ProductDto> UpdateAsync(int craftsmanId, int productId, UpdateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product is null)
                throw new KeyNotFoundException("Product not found.");

            if (product.CraftsmanId != craftsmanId)
                throw new UnauthorizedAccessException("You can only edit your own products.");

            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category is null)
                throw new InvalidOperationException("Category not found.");

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.CategoryId = dto.CategoryId;
            product.EmbroideryStyle = dto.EmbroideryStyle;
            product.Colors = dto.Colors;
            product.IsCustomizable = dto.IsCustomizable;
            product.EstimatedProductionDays = dto.EstimatedProductionDays;

            // أي تعديل على منتج منشور يرجعه لمراجعة الأدمن تاني
            // أي تعديل على منتج منشور يرجعه لمراجعة الأدمن تاني
            // (والحرفي المعلّق بتضل أعماله مسودة لحد ما يتقبل)
            var craftsman = await _craftsmanRepository.GetByUserIdAsync(craftsmanId);
            product.Status = craftsman?.Status == CraftsmanStatus.Approved
                ? ProductStatus.PendingReview
                : ProductStatus.Draft;
            await _productRepository.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task DeleteAsync(int craftsmanId, int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product is null)
                throw new KeyNotFoundException("Product not found.");

            if (product.CraftsmanId != craftsmanId)
                throw new UnauthorizedAccessException("You can only delete your own products.");

            _productRepository.Remove(product);
            await _productRepository.SaveChangesAsync();
        }

        private static ProductDto MapToDto(Product p) => new()
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
            Images = p.Images.Select(i => new ProductImageDto
            {
                Id = i.Id,
                Url = i.Url,
                IsMain = i.IsMain
            }).ToList()
        };
    }
}
