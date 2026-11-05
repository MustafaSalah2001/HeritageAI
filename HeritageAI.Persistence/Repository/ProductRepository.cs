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
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(int id) =>
            await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Craftsman).ThenInclude(c => c.User)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
        public async Task<List<Product>> GetByCraftsmanIdAsync(int craftsmanId) =>
            await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Craftsman).ThenInclude(c => c.User)
                .Include(p => p.Images)
                .Where(p => p.CraftsmanId == craftsmanId)
                .ToListAsync(); public async Task<List<Product>> GetPendingAsync() =>
    await _context.Products
        .Include(p => p.Category)
        .Include(p => p.Craftsman).ThenInclude(c => c.User)
        .Include(p => p.Images)
        .Where(p => p.Status == ProductStatus.PendingReview)
        .ToListAsync();
        public async Task<List<Product>> GetPublishedAsync() =>
            await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Craftsman).ThenInclude(c => c.User)
                .Include(p => p.Images)
.Where(p => p.Status == ProductStatus.Published && p.Craftsman.User.IsActive).ToListAsync();
        public async Task<List<Product>> GetAllAsync() =>
    await _context.Products
        .Include(p => p.Category)
        .Include(p => p.Craftsman).ThenInclude(c => c.User)
        .Include(p => p.Images)
        .OrderByDescending(p => p.CreatedAt)
        .ToListAsync();
        public async Task AddAsync(Product product) =>
            await _context.Products.AddAsync(product);

        public void Remove(Product product) =>
            _context.Products.Remove(product);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}
