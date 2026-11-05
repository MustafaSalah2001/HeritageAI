namespace HeritageAI.Application.Dto.Review
{
    public class CreateReviewDto
    {
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }

    public class ProductReviewDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string BuyerName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ProductReviewsResponseDto
    {
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public List<ProductReviewDto> Reviews { get; set; } = new();
    }

    public class MyReviewDto
    {
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}