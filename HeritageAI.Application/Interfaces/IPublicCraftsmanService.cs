using HeritageAI.Application.Dto.Craftsman;
using HeritageAI.Application.Dto.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Interfaces
{
    public interface IPublicCraftsmanService
    {
        Task<List<CraftsmanPublicProfileDto>> GetAllApprovedAsync();   // ← جديد
        Task<CraftsmanPublicProfileDto> GetPublicProfileAsync(int craftsmanId);
        Task<List<ProductDto>> GetCraftsmanProductsAsync(int craftsmanId);
    }
}
