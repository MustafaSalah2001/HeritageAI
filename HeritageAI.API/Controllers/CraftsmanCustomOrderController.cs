using HeritageAI.Application.Dto.CustomOrder;
using HeritageAI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HeritageAI.API.Controllers
{
    [ApiController]
    [Route("api/craftsman/custom-orders")]
    [Authorize(Roles = "Craftsman")]
    public class CraftsmanCustomOrderController : BaseController
    {
        private readonly ICustomOrderService _service;

        public CraftsmanCustomOrderController(ICustomOrderService service)
        {
            _service = service;
        }

       

        [HttpGet("mine")]
        public async Task<IActionResult> GetMyRequests()
        {
            var craftsmanId = GetCurrentUserId();
            var result = await _service.GetMyRequestsAsCraftsmanAsync(craftsmanId);
            return Ok(result);
        }

        [HttpPut("{id}/accept")]
        public async Task<IActionResult> Accept(int id, RespondCustomOrderDto dto)
        {
            try
            {
                var craftsmanId = GetCurrentUserId();
                var result = await _service.AcceptAsync(craftsmanId, id, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("{id}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                var craftsmanId = GetCurrentUserId();
                var result = await _service.RejectAsync(craftsmanId, id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }
    }
}
