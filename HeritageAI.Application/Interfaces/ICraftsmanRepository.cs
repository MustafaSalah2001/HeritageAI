using HeritageAI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Interfaces
{
    public interface ICraftsmanRepository
    {
        Task AddAsync(Craftsman craftsman);
        Task<Craftsman?> GetByUserIdAsync(int userId);
        Task<List<Craftsman>> GetPendingAsync();
        Task<List<Craftsman>> GetAllAsync();
        Task SaveChangesAsync();
    }
}
