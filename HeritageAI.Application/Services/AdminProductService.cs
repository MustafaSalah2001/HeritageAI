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
    public class AdminProductService : IAdminProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly INotificationRepository _notifications;
        private const string CraftsmanHome = "/craftsman/dashboard";

        public AdminProductService(
            IProductRepository productRepository,
            INotificationRepository notifications)
        {
            _productRepository = productRepository;
            _notifications = notifications;
        }

        public async Task<List<ProductDto>> GetPendingAsync()
        {
            var products = await _productRepository.GetPendingAsync();
            return products.Select(MapToDto).ToList();
        }

        public async Task<List<ProductDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(MapToDto).ToList();
        }

        public async Task ApproveAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product is null)
                throw new KeyNotFoundException("Product not found.");

            var changed = product.Status != ProductStatus.Published;
            product.Status = ProductStatus.Published;

            if (changed)
            {
                _notifications.Add(new Notification
                {
                    UserId = product.CraftsmanId,
                    Title = "تم نشر منتجك",
                    Message = $"وافقت الإدارة على منتجك \"{product.Name}\" وصار ظاهر بالمتجر.",
                    Link = CraftsmanHome
                });
            }

            await _productRepository.SaveChangesAsync();
        }

        public async Task RejectAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product is null)
                throw new KeyNotFoundException("Product not found.");

            var changed = product.Status != ProductStatus.Rejected;
            product.Status = ProductStatus.Rejected;

            if (changed)
            {
                _notifications.Add(new Notification
                {
                    UserId = product.CraftsmanId,
                    Title = "تم رفض منتجك",
                    Message = $"رفضت الإدارة المنتج \"{product.Name}\". عدّله وأعد إرساله للمراجعة.",
                    Link = CraftsmanHome
                });
            }

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