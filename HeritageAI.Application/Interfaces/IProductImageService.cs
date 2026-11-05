using HeritageAI.Application.Dto.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Interfaces
{
    public interface IProductImageService
    {
        Task<ProductImageDto> UploadImageAsync(int craftsmanId, int productId, Stream fileStream, string fileName);
        Task DeleteImageAsync(int craftsmanId, int imageId);
        Task SetMainImageAsync(int craftsmanId, int imageId);
    }
}
