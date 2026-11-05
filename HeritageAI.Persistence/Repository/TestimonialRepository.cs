using HeritageAI.Application.Interfaces;
using HeritageAI.Domain.Entities;
using HeritageAI.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeritageAI.Persistence.Repository
{
    public class TestimonialRepository : ITestimonialRepository
    {
        private readonly AppDbContext _context;

        public TestimonialRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Testimonial?> GetByIdAsync(int id) =>
            await _context.Testimonials
                .Include(t => t.Buyer)
                .FirstOrDefaultAsync(t => t.Id == id);

        public async Task<List<Testimonial>> GetApprovedAsync() =>
            await _context.Testimonials
                .Include(t => t.Buyer)
                .Where(t => t.Status == TestimonialStatus.Approved)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

        public async Task<List<Testimonial>> GetPendingAsync() =>
            await _context.Testimonials
                .Include(t => t.Buyer)
                .Where(t => t.Status == TestimonialStatus.Pending)
                .OrderBy(t => t.CreatedAt)
                .ToListAsync();

        public async Task<List<Testimonial>> GetAllAsync() =>
            await _context.Testimonials
                .Include(t => t.Buyer)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

        public async Task<List<Testimonial>> GetByBuyerIdAsync(int buyerId) =>
            await _context.Testimonials
                .Include(t => t.Buyer)
                .Where(t => t.BuyerId == buyerId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

        public async Task AddAsync(Testimonial testimonial) =>
            await _context.Testimonials.AddAsync(testimonial);

        public void Remove(Testimonial testimonial) =>
            _context.Testimonials.Remove(testimonial);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}