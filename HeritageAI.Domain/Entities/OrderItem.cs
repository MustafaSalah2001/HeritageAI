using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public string? CustomizationNotes { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; } // Snapshot من سعر المنتج وقت الطلب
    }
}
