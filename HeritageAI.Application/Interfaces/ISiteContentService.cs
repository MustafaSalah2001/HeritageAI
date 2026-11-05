using HeritageAI.Application.Dto.SiteContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Interfaces
{
    public interface ISiteContentService
    {
        Task<SiteContentDto> GetAsync();
        Task<SiteContentDto> UpdateAsync(UpdateSiteContentDto dto);
        Task<HomepageImageDto> AddImageAsync(Stream fileStream, string fileName);
        Task DeleteImageAsync(int imageId);
    }
}
