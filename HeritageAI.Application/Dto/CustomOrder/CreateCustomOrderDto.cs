using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Dto.CustomOrder
{
    public class CreateCustomOrderDto
    {
        public int CraftsmanId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? DesiredColors { get; set; }
        public string? Size { get; set; }
    }
}
