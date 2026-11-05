using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Dto.SiteContent
{
    public class SiteContentDto
    {
        public string AboutUsText { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string? FacebookUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public List<HomepageImageDto> HomepageImages { get; set; } = new();
    }
}
