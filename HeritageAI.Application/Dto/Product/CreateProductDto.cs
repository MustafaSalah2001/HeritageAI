using HeritageAI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Dto.Product
{
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public EmbroideryStyle EmbroideryStyle { get; set; }
        public string Colors { get; set; } = string.Empty;
        public bool IsCustomizable { get; set; }
        public int EstimatedProductionDays { get; set; }
    }
}
