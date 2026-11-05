using HeritageAI.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeritageAI.Application.Interfaces
{
    public interface ITestimonialRepository
    {
        Task<Testimonial?> GetByIdAsync(int id);
        Task<List<Testimonial>> GetApprovedAsync();
        Task<List<Testimonial>> GetPendingAsync();
        Task<List<Testimonial>> GetAllAsync();
        Task<List<Testimonial>> GetByBuyerIdAsync(int buyerId);
        Task AddAsync(Testimonial testimonial);
        void Remove(Testimonial testimonial);
        Task SaveChangesAsync(); 
    }
}