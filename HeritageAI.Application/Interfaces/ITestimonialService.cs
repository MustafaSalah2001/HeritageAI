using HeritageAI.Application.Dto.Testimonial;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeritageAI.Application.Interfaces
{
    public interface ITestimonialService
    {
        Task<TestimonialDto> CreateAsync(int buyerId, CreateTestimonialDto dto);
        Task<List<TestimonialDto>> GetMyTestimonialsAsync(int buyerId);
        Task<List<TestimonialDto>> GetApprovedAsync();

        // Admin
        Task<List<TestimonialDto>> GetPendingAsync();
        Task<List<TestimonialDto>> GetAllAsync();
        Task<TestimonialDto> ApproveAsync(int id);
        Task<TestimonialDto> RejectAsync(int id);
        Task DeleteAsync(int id); 
    }
}