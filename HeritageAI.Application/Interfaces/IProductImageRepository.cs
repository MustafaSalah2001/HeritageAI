using HeritageAI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Persistence.Repository
{
    public interface IProductImageRepository
    {
        Task<ProductImage?> GetByIdAsync(int id);
        Task AddAsync(ProductImage image);
        void Remove(ProductImage image);
        Task<List<ProductImage>> GetByProductIdAsync(int productId);
        Task SaveChangesAsync();
    }
}
