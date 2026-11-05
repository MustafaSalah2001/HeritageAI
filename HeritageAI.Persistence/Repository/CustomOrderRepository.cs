using HeritageAI.Application.Interfaces;
using HeritageAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Persistence.Repository
{
    public class CustomOrderRepository : ICustomOrderRepository
    {
        private readonly AppDbContext _context;

        public CustomOrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CustomOrderRequest?> GetByIdAsync(int id) =>
            await _context.CustomOrderRequests
                .Include(c => c.Buyer)
                .Include(c => c.Craftsman).ThenInclude(cr => cr.User)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<List<CustomOrderRequest>> GetByBuyerIdAsync(int buyerId) =>
            await _context.CustomOrderRequests
                .Include(c => c.Buyer)
                .Include(c => c.Craftsman).ThenInclude(cr => cr.User)
                .Where(c => c.BuyerId == buyerId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

        public async Task<List<CustomOrderRequest>> GetByCraftsmanIdAsync(int craftsmanId) =>
            await _context.CustomOrderRequests
                .Include(c => c.Buyer)
                .Include(c => c.Craftsman).ThenInclude(cr => cr.User)
                .Where(c => c.CraftsmanId == craftsmanId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

        public async Task AddAsync(CustomOrderRequest request) =>
            await _context.CustomOrderRequests.AddAsync(request);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}
