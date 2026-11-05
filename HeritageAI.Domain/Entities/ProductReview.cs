namespace HeritageAI.Domain.Entities
{
    public class ProductReview
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int BuyerId { get; set; }
        public User Buyer { get; set; } = null!;
        public int Rating { get; set; }          // 1..5
        public string? Comment { get; set; }     // اختياري، حتى 500 حرف
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}