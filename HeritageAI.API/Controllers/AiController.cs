using HeritageAI.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeritageAI.API.Controllers
{
    [ApiController]
    [Route("api/ai")]
    public class AiController : ControllerBase
    {
        private readonly IAiDesignService _service;

        public AiController(IAiDesignService service)
        {
            _service = service;
        }

        // للمشتري المسجّل بس، عشان ما حدا يحرق الرصيد
        [Authorize(Roles = "Buyer")]
        [HttpPost("design")]
        public async Task<IActionResult> Design(AiDesignRequestDto dto)
        {
            try
            {
                return Ok(await _service.GenerateAsync(dto));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}