using HeritageAI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeritageAI.API.Controllers
{
    [ApiController]
    [Route("api/admin/craftsmen")]
    [Authorize(Roles = "Admin")]
    public class AdminCraftsmanController : ControllerBase
    {
        private readonly IAdminCraftsmanService _service;

        public AdminCraftsmanController(IAdminCraftsmanService service)
        {
            _service = service;
        }
        [HttpGet("{userId:int}")]
        public async Task<IActionResult> GetDetails(int userId)
        {
            var result = await _service.GetDetailsAsync(userId);
            return result is null
                ? NotFound(new { message = "Craftsman not found." })
                : Ok(result);
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            var result = await _service.GetPendingAsync();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpPut("{userId}/approve")]
        public async Task<IActionResult> Approve(int userId)
        {
            try
            {
                await _service.ApproveAsync(userId);
                return Ok(new { message = "Craftsman approved." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("{userId}/reject")]
        public async Task<IActionResult> Reject(int userId)
        {
            try
            {
                await _service.RejectAsync(userId);
                return Ok(new { message = "Craftsman rejected." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
