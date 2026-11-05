using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Dto.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? RejectionReason { get; set; }
        public bool CancelledByBuyer { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
