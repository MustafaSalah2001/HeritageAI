using System;

namespace HeritageAI.Domain.Entities
{
    public class OrderTransfer
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int FromCraftsmanId { get; set; }
        public int ToCraftsmanId { get; set; }
        public int AdminId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}