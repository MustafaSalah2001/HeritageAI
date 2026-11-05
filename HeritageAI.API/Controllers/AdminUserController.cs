using HeritageAI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeritageAI.API.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "Admin")]
    public class AdminUserController : ControllerBase
    {
        private readonly IAdminUserService _service;

        public AdminUserController(IAdminUserService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpPut("{userId}/deactivate")]
        public async Task<IActionResult> Deactivate(int userId)
        {
            try
            {
                await _service.SetActiveStatusAsync(userId, false);
                return Ok(new { message = "User deactivated." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{userId}/activate")]
        public async Task<IActionResult> Activate(int userId)
        {
            try
            {
                await _service.SetActiveStatusAsync(userId, true);
                return Ok(new { message = "User activated." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
