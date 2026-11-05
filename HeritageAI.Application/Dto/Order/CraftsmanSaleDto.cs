using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Dto.Order
{
    public class CraftsmanSaleDto
    {
        public int OrderId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal LineTotal { get; set; }
        public string? CustomizationNotes { get; set; }
        public string BuyerName { get; set; } = string.Empty;
        public string? ShippingCity { get; set; }
        public string? ShippingAddress { get; set; }
        public string? ContactPhone { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
    }
}
