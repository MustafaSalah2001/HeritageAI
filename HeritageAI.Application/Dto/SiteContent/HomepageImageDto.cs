using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Dto.SiteContent
{
    public class HomepageImageDto
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
