namespace HeritageAI.Application.Dto.Order
{
    public class ReassignOrderDto
    {
        public int NewCraftsmanId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}