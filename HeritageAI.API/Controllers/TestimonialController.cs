using HeritageAI.Application.Dto.Testimonial;
using HeritageAI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeritageAI.API.Controllers
{
    [ApiController]
    [Route("api/testimonials")]
    [Authorize(Roles = "Buyer")]
    public class TestimonialController : BaseController
    {
        private readonly ITestimonialService _service;

        public TestimonialController(ITestimonialService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTestimonialDto dto)
        {
            try
            {
                var buyerId = GetCurrentUserId();
                var result = await _service.CreateAsync(buyerId, dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var buyerId = GetCurrentUserId();
            var result = await _service.GetMyTestimonialsAsync(buyerId);
            return Ok(result);
        }
    }
}