using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeritageAI.Domain.Enums;

namespace HeritageAI.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public int CraftsmanId { get; set; }
        public Craftsman Craftsman { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public EmbroideryStyle EmbroideryStyle { get; set; }
        public string Colors { get; set; } = string.Empty; // MVP: comma-separated, نبسطها الأول
        public bool IsCustomizable { get; set; }
        public int EstimatedProductionDays { get; set; }

        public ProductStatus Status { get; set; } = ProductStatus.PendingReview;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    }
}
