using HeritageAI.Application.Dto.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Interfaces
{
    public interface IAdminProductService
    {
        Task<List<ProductDto>> GetPendingAsync();
        Task ApproveAsync(int productId);
        Task<List<ProductDto>> GetAllAsync();
        Task RejectAsync(int productId);
    }
}
