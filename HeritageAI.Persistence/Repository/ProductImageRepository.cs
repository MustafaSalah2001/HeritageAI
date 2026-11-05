using HeritageAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Persistence.Repository
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly AppDbContext _context;

        public ProductImageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductImage?> GetByIdAsync(int id) =>
            await _context.ProductImages.FirstOrDefaultAsync(i => i.Id == id);

        public async Task AddAsync(ProductImage image) =>
            await _context.ProductImages.AddAsync(image);

        public void Remove(ProductImage image) =>
            _context.ProductImages.Remove(image);

        public async Task<List<ProductImage>> GetByProductIdAsync(int productId) =>
            await _context.ProductImages.Where(i => i.ProductId == productId).ToListAsync();

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}
