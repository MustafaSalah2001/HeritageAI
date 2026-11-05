namespace HeritageAI.Application.Dto.Testimonial
{
    public class CreateTestimonialDto
    {
        public string Content { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Location { get; set; }
    }
}