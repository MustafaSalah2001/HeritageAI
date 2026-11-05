using HeritageAI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Persistence.Repository
{
    public interface ISiteContentRepository
    {
        Task<SiteContent?> GetAsync();
        Task AddAsync(SiteContent content);
        Task<List<HomepageImage>> GetImagesAsync();
        Task<HomepageImage?> GetImageByIdAsync(int id);
        Task AddImageAsync(HomepageImage image);
        void RemoveImage(HomepageImage image);
        Task SaveChangesAsync();
    }
}
