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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync() =>
            await _context.Categories.ToListAsync();

        public async Task<Category?> GetByIdAsync(int id) =>
            await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Category?> GetByNameAsync(string name) =>
            await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);

        public async Task AddAsync(Category category) =>
            await _context.Categories.AddAsync(category);

        public void Remove(Category category) =>
            _context.Categories.Remove(category);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}
