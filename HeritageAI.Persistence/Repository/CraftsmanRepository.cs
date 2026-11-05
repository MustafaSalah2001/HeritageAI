using HeritageAI.Application.Interfaces;
using HeritageAI.Domain.Entities;
using HeritageAI.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Persistence.Repository
{
    public class CraftsmanRepository : ICraftsmanRepository
    {
        private readonly AppDbContext _context;

        public CraftsmanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Craftsman craftsman) =>
            await _context.Craftsmen.AddAsync(craftsman);

        public async Task<Craftsman?> GetByUserIdAsync(int userId) =>
     await _context.Craftsmen
         .Include(c => c.User)
         .Include(c => c.Skills)   // ← جديد
         .FirstOrDefaultAsync(c => c.UserId == userId);

        public async Task<List<Craftsman>> GetPendingAsync() =>
            await _context.Craftsmen
                .Include(c => c.User)
                .Include(c => c.Skills)
                .Where(c => c.Status == CraftsmanStatus.Pending)
                .ToListAsync();
        public async Task<List<Craftsman>> GetAllAsync() =>
            await _context.Craftsmen
                .Include(c => c.User)
                .ToListAsync();

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}
