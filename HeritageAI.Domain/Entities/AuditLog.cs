using System;

namespace HeritageAI.Domain.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int AdminId { get; set; }
        public string Action { get; set; } = string.Empty;      // OrderAccepted / OrderRejected ...
        public string EntityType { get; set; } = string.Empty;  // Order
        public int EntityId { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}