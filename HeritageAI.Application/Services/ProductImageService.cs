using HeritageAI.Application.Dto.Product;
using HeritageAI.Application.Interfaces;
using HeritageAI.Domain.Entities;
using HeritageAI.Persistence.Repository;
using System.IO;   // عشان Stream
//using HeritageAI.Persistence.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductImageRepository _imageRepository;
        private readonly IFileStorageService _fileStorage;

        public ProductImageService(
            IProductRepository productRepository,
            IProductImageRepository imageRepository,
            IFileStorageService fileStorage)
        {
            _productRepository = productRepository;
            _imageRepository = imageRepository;
            _fileStorage = fileStorage;
        }

        public async Task<ProductImageDto> UploadImageAsync(int craftsmanId, int productId, Stream fileStream, string fileName)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product is null)
                throw new KeyNotFoundException("Product not found.");

            if (product.CraftsmanId != craftsmanId)
                throw new UnauthorizedAccessException("You can only add images to your own products.");

            var url = await _fileStorage.SaveProductImageAsync(fileStream, fileName);

            var isFirstImage = !(await _imageRepository.GetByProductIdAsync(productId)).Any();

            var image = new ProductImage
            {
                ProductId = productId,
                Url = url,
                IsMain = isFirstImage   // أول صورة بتتحط تلقائيًا Main
            };

            await _imageRepository.AddAsync(image);
            await _imageRepository.SaveChangesAsync();

            return new ProductImageDto { Id = image.Id, Url = image.Url, IsMain = image.IsMain };
        }

        public async Task DeleteImageAsync(int craftsmanId, int imageId)
        {
            var image = await _imageRepository.GetByIdAsync(imageId);

            if (image is null)
                throw new KeyNotFoundException("Image not found.");

            var product = await _productRepository.GetByIdAsync(image.ProductId);

            if (product is null || product.CraftsmanId != craftsmanId)
                throw new UnauthorizedAccessException("You can only delete images from your own products.");

            _fileStorage.DeleteFile(image.Url);
            _imageRepository.Remove(image);
            await _imageRepository.SaveChangesAsync();
        }

        public async Task SetMainImageAsync(int craftsmanId, int imageId)
        {
            var image = await _imageRepository.GetByIdAsync(imageId);

            if (image is null)
                throw new KeyNotFoundException("Image not found.");

            var product = await _productRepository.GetByIdAsync(image.ProductId);

            if (product is null || product.CraftsmanId != craftsmanId)
                throw new UnauthorizedAccessException("You can only edit your own products.");

            var allImages = await _imageRepository.GetByProductIdAsync(image.ProductId);

            foreach (var img in allImages)
                img.IsMain = (img.Id == imageId);

            await _imageRepository.SaveChangesAsync();
        }
    }
}
