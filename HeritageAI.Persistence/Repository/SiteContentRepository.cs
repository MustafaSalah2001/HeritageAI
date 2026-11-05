using HeritageAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Persistence.Repository
{
    public class SiteContentRepository : ISiteContentRepository
    {
        private readonly AppDbContext _context;

        public SiteContentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SiteContent?> GetAsync() =>
            await _context.SiteContents.FirstOrDefaultAsync();

        public async Task AddAsync(SiteContent content) =>
            await _context.SiteContents.AddAsync(content);

        public async Task<List<HomepageImage>> GetImagesAsync() =>
            await _context.HomepageImages.OrderBy(h => h.DisplayOrder).ToListAsync();

        public async Task<HomepageImage?> GetImageByIdAsync(int id) =>
            await _context.HomepageImages.FirstOrDefaultAsync(h => h.Id == id);

        public async Task AddImageAsync(HomepageImage image) =>
            await _context.HomepageImages.AddAsync(image);

        public void RemoveImage(HomepageImage image) =>
            _context.HomepageImages.Remove(image);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}
