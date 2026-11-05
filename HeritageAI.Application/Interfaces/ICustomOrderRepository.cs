using HeritageAI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Interfaces
{
    public interface ICustomOrderRepository
    {
        Task<CustomOrderRequest?> GetByIdAsync(int id);
        Task<List<CustomOrderRequest>> GetByBuyerIdAsync(int buyerId);
        Task<List<CustomOrderRequest>> GetByCraftsmanIdAsync(int craftsmanId);
        Task AddAsync(CustomOrderRequest request);
        Task SaveChangesAsync();
    }
}
