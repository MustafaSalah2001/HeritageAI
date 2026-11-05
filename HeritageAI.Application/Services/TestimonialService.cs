using HeritageAI.Application.Dto.Testimonial;
using HeritageAI.Application.Interfaces;
using HeritageAI.Domain.Entities;
using HeritageAI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeritageAI.Application.Services
{
    public class TestimonialService : ITestimonialService
    {
        private readonly ITestimonialRepository _repository;
        private readonly INotificationRepository _notifications;

        public TestimonialService(
            ITestimonialRepository repository,
            INotificationRepository notifications)
        {
            _repository = repository;
            _notifications = notifications;
        }

        public async Task<TestimonialDto> CreateAsync(int buyerId, CreateTestimonialDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Content))
                throw new InvalidOperationException("Review text is required.");

            if (dto.Content.Length > 600)
                throw new InvalidOperationException("Review text is too long.");

            if (dto.Rating < 1 || dto.Rating > 5)
                throw new InvalidOperationException("Rating must be between 1 and 5.");

            var testimonial = new Testimonial
            {
                BuyerId = buyerId,
                Content = dto.Content.Trim(),
                Rating = dto.Rating,
                Location = string.IsNullOrWhiteSpace(dto.Location) ? null : dto.Location.Trim(),
                Status = TestimonialStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(testimonial);
            await _notifications.AddForAdminsAsync(
    "رأي جديد بانتظار المراجعة",
    "وصل رأي جديد من أحد العملاء، راجعه واعتمده أو ارفضه.",
    "/admin/testimonials");
            await _repository.SaveChangesAsync();

            var created = await _repository.GetByIdAsync(testimonial.Id);
            return MapToDto(created!);
        }

        public async Task<List<TestimonialDto>> GetMyTestimonialsAsync(int buyerId)
        {
            var items = await _repository.GetByBuyerIdAsync(buyerId);
            return items.Select(MapToDto).ToList();
        }

        public async Task<List<TestimonialDto>> GetApprovedAsync()
        {
            var items = await _repository.GetApprovedAsync();
            return items.Select(MapToDto).ToList();
        }

        public async Task<List<TestimonialDto>> GetPendingAsync()
        {
            var items = await _repository.GetPendingAsync();
            return items.Select(MapToDto).ToList();
        }

        public async Task<List<TestimonialDto>> GetAllAsync()
        {
            var items = await _repository.GetAllAsync();
            return items.Select(MapToDto).ToList();
        }

        public async Task<TestimonialDto> ApproveAsync(int id)
        {
            var testimonial = await _repository.GetByIdAsync(id);
            if (testimonial is null)
                throw new KeyNotFoundException("Testimonial not found.");

            testimonial.Status = TestimonialStatus.Approved;
            await _repository.SaveChangesAsync();
            return MapToDto(testimonial);
        }

        public async Task<TestimonialDto> RejectAsync(int id)
        {
            var testimonial = await _repository.GetByIdAsync(id);
            if (testimonial is null)
                throw new KeyNotFoundException("Testimonial not found.");

            testimonial.Status = TestimonialStatus.Rejected;
            await _repository.SaveChangesAsync();

            return MapToDto(testimonial);
        }

        public async Task DeleteAsync(int id)
        {
            var testimonial = await _repository.GetByIdAsync(id);

            if (testimonial is null)
                throw new KeyNotFoundException("Testimonial not found.");

            _repository.Remove(testimonial);
            await _repository.SaveChangesAsync();
        }

        private static TestimonialDto MapToDto(Testimonial t) => new()
        {
            Id = t.Id,
            BuyerId = t.BuyerId,
            BuyerName = t.Buyer.FullName,
            Content = t.Content,
            Rating = t.Rating,
            Location = t.Location,
            Status = t.Status.ToString(),
            CreatedAt = t.CreatedAt
        };
    }
}