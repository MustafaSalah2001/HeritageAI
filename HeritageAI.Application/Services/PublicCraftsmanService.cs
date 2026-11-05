using HeritageAI.Application.Dto.Craftsman;
using HeritageAI.Application.Dto.Product;
using HeritageAI.Application.Interfaces;
using HeritageAI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Services
{
    public class PublicCraftsmanService : IPublicCraftsmanService
    {
        private readonly ICraftsmanRepository _craftsmanRepository;
        private readonly IProductRepository _productRepository;

        public PublicCraftsmanService(
            ICraftsmanRepository craftsmanRepository,
            IProductRepository productRepository)
        {
            _craftsmanRepository = craftsmanRepository;
            _productRepository = productRepository;
        }

        public async Task<CraftsmanPublicProfileDto> GetPublicProfileAsync(int craftsmanId)
        {
            var craftsman = await _craftsmanRepository.GetByUserIdAsync(craftsmanId);

            
            if (craftsman is null || craftsman.Status != CraftsmanStatus.Approved || !craftsman.User.IsActive)
                throw new KeyNotFoundException("Craftsman not found.");
            return new CraftsmanPublicProfileDto
            {
                UserId = craftsman.UserId,
                FullName = craftsman.User.FullName,
                Bio = craftsman.Bio,
                StoryText = craftsman.StoryText,
                ProfileImageUrl = craftsman.ProfileImageUrl,
                Skills = craftsman.Skills.Select(s => s.Name).ToList()
            };
        }

        public async Task<List<ProductDto>> GetCraftsmanProductsAsync(int craftsmanId)
        {
            var products = await _productRepository.GetByCraftsmanIdAsync(craftsmanId);

            return products
                .Where(p => p.Status == ProductStatus.Published && p.Craftsman.User.IsActive)
                .Select(p => new ProductDto
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
                }).ToList();
        }
        public async Task<List<CraftsmanPublicProfileDto>> GetAllApprovedAsync()
        {
            var craftsmen = await _craftsmanRepository.GetAllAsync();

                       return craftsmen
                .Where(c => c.Status == CraftsmanStatus.Approved && c.User.IsActive)
                .Select(c => new CraftsmanPublicProfileDto
                {
                    UserId = c.UserId,
                    FullName = c.User.FullName,
                    Bio = c.Bio,
                    StoryText = c.StoryText,
                    ProfileImageUrl = c.ProfileImageUrl,
                    Skills = c.Skills != null ? c.Skills.Select(s => s.Name).ToList() : new List<string>()
                }).ToList();
        }
    }
}
