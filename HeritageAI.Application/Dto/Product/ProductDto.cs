using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Dto.Product
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string EmbroideryStyle { get; set; } = string.Empty;
        public string Colors { get; set; } = string.Empty;
        public bool IsCustomizable { get; set; }
        public int EstimatedProductionDays { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CraftsmanName { get; set; } = string.Empty;
        public int CraftsmanId { get; set; }
        public List<ProductImageDto> Images { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
