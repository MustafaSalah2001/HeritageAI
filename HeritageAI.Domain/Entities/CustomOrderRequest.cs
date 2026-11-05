using HeritageAI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Domain.Entities
{
    public class CustomOrderRequest
    {
        public int Id { get; set; }

        public int BuyerId { get; set; }
        public User Buyer { get; set; } = null!;

        public int CraftsmanId { get; set; }
        public Craftsman Craftsman { get; set; } = null!;

        public string Description { get; set; } = string.Empty;
        public string? DesiredColors { get; set; }
        public string? Size { get; set; }

        public CustomOrderStatus Status { get; set; } = CustomOrderStatus.Pending;
        public decimal? ProposedPrice { get; set; }
        public int? EstimatedDurationDays { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
