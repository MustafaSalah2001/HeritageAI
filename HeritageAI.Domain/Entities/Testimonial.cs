using HeritageAI.Domain.Enums;
using System;

namespace HeritageAI.Domain.Entities
{
    public class Testimonial
    {
        public int Id { get; set; }

        public int BuyerId { get; set; }
        public User Buyer { get; set; } = null!;

        public string Content { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Location { get; set; }

        public TestimonialStatus Status { get; set; } = TestimonialStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}