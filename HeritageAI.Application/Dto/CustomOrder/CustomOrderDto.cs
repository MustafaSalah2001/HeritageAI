using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Dto.CustomOrder
{
    public class CustomOrderDto
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public string BuyerName { get; set; } = string.Empty;
        public int CraftsmanId { get; set; }
        public string CraftsmanName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? DesiredColors { get; set; }
        public string? Size { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal? ProposedPrice { get; set; }
        public int? EstimatedDurationDays { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
