using HeritageAI.Application.Dto.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetMyProductsAsync(int craftsmanId);
        Task<ProductDto> CreateAsync(int craftsmanId, CreateProductDto dto);
        Task<ProductDto> UpdateAsync(int craftsmanId, int productId, UpdateProductDto dto);
        Task DeleteAsync(int craftsmanId, int productId);
    }
}
