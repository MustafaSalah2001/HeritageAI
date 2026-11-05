using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeritageAI.Domain.Enums;

namespace HeritageAI.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int BuyerId { get; set; } // FK → User.Id (مش Buyer منفصل)
        public User Buyer { get; set; } = null!;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string? RejectionReason { get; set; }
        public bool CancelledByBuyer { get; set; }
        public decimal TotalPrice { get; set; }
        // true لو الإدارة هي اللي قبلت/رفضت الطلب بدل الحرفي
        public bool HandledByAdmin { get; set; }
        public string? ShippingCity { get; set; }
        public string? ShippingAddress { get; set; }
        // لو الأدمن نقل الطلب لحرفي ثاني. null = الحرفي صاحب المنتج الأصلي
        public int? AssignedCraftsmanId { get; set; }
        public Craftsman? AssignedCraftsman { get; set; }
        public string? ContactPhone { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
