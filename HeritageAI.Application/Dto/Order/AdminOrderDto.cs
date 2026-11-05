using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Dto.Order
{
    // نسخة موسّعة من OrderDto مخصوصة للوحة الأدمن، فيها بيانات المشتري
    // عشان الأدمن يقدر يراقب كل الطلبات على المنصة.
    public class AdminOrderDto
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool HandledByAdmin { get; set; }
        public decimal TotalPrice { get; set; }
        public int CraftsmanId { get; set; }
        public bool IsReassigned { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CraftsmanName { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
        public bool CancelledByBuyer { get; set; }
        public string BuyerName { get; set; } = string.Empty;
        public string BuyerEmail { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new();
    }
}