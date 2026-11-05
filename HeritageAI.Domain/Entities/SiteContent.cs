using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Domain.Entities
{
    public class SiteContent
    {
        public int Id { get; set; }
        public string AboutUsText { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string? FacebookUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? HeroImageUrl { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
