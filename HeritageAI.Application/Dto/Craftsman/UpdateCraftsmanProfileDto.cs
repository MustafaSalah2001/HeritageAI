namespace HeritageAI.Application.Dto.Craftsman
{
    public class UpdateCraftsmanProfileDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string? StoryText { get; set; }
    }
}