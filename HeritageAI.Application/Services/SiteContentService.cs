using HeritageAI.Application.Dto.SiteContent;
using HeritageAI.Application.Interfaces;
using HeritageAI.Domain.Entities;
using HeritageAI.Persistence.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Services
{
    public class SiteContentService : ISiteContentService
    {
        private readonly ISiteContentRepository _repository;
        private readonly IFileStorageService _fileStorage;

        public SiteContentService(
            ISiteContentRepository repository,
            IFileStorageService fileStorage)
        {
            _repository = repository;
            _fileStorage = fileStorage;
        }

        public async Task<SiteContentDto> GetAsync()
        {
            var content = await _repository.GetAsync();

            // لو مفيش محتوى خالص لسه (أول مرة)، رجّع نسخة فاضية بدل Error
            if (content is null)
            {
                return new SiteContentDto();
            }

            var images = await _repository.GetImagesAsync();

            return MapToDto(content, images);
        }

        public async Task<SiteContentDto> UpdateAsync(UpdateSiteContentDto dto)
        {
            var content = await _repository.GetAsync();

            if (content is null)
            {
                // أول مرة يتم فيها تعديل المحتوى — ننشئ الصف لأول مرة
                content = new SiteContent
                {
                    AboutUsText = dto.AboutUsText,
                    ContactEmail = dto.ContactEmail,
                    ContactPhone = dto.ContactPhone,
                    FacebookUrl = dto.FacebookUrl,
                    InstagramUrl = dto.InstagramUrl,
                    UpdatedAt = DateTime.UtcNow
                };
                await _repository.AddAsync(content);
            }
            else
            {
                content.AboutUsText = dto.AboutUsText;
                content.ContactEmail = dto.ContactEmail;
                content.ContactPhone = dto.ContactPhone;
                content.FacebookUrl = dto.FacebookUrl;
                content.InstagramUrl = dto.InstagramUrl;
                content.UpdatedAt = DateTime.UtcNow;
            }

            await _repository.SaveChangesAsync();

            var images = await _repository.GetImagesAsync();
            return MapToDto(content, images);
        }

        public async Task<HomepageImageDto> AddImageAsync(Stream fileStream, string fileName)
        {
            var url = await _fileStorage.SaveProductImageAsync(fileStream, fileName);

            var currentImages = await _repository.GetImagesAsync();
            var nextOrder = currentImages.Count == 0 ? 1 : currentImages.Max(i => i.DisplayOrder) + 1;

            var image = new HomepageImage
            {
                Url = url,
                DisplayOrder = nextOrder,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddImageAsync(image);
            await _repository.SaveChangesAsync();

            return new HomepageImageDto { Id = image.Id, Url = image.Url, DisplayOrder = image.DisplayOrder };
        }

        public async Task DeleteImageAsync(int imageId)
        {
            var image = await _repository.GetImageByIdAsync(imageId);

            if (image is null)
                throw new KeyNotFoundException("Image not found.");

            _fileStorage.DeleteFile(image.Url);
            _repository.RemoveImage(image);
            await _repository.SaveChangesAsync();
        }

        private static SiteContentDto MapToDto(SiteContent content, List<HomepageImage> images) => new()
        {
            AboutUsText = content.AboutUsText,
            ContactEmail = content.ContactEmail,
            ContactPhone = content.ContactPhone,
            FacebookUrl = content.FacebookUrl,
            InstagramUrl = content.InstagramUrl,
            HomepageImages = images.Select(i => new HomepageImageDto
            {
                Id = i.Id,
                Url = i.Url,
                DisplayOrder = i.DisplayOrder
            }).ToList()
        };
    }
}
