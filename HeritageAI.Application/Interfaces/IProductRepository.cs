using HeritageAI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<List<Product>> GetByCraftsmanIdAsync(int craftsmanId);
        Task<List<Product>> GetPendingAsync();
        Task<List<Product>> GetAllAsync();
        Task<List<Product>> GetPublishedAsync();
        Task AddAsync(Product product);
        void Remove(Product product);
        Task SaveChangesAsync();
    }
}
