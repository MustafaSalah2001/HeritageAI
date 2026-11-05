using HeritageAI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HeritageAI.API.Controllers
{
    [ApiController]
    [Route("api/public/testimonials")]
    public class PublicTestimonialController : ControllerBase
    {
        private readonly ITestimonialService _service;

        public PublicTestimonialController(ITestimonialService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetApproved()
        {
            var result = await _service.GetApprovedAsync();
            return Ok(result);
        }
    }
}