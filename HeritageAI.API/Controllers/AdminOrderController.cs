using HeritageAI.Application.Dto.Order;
using HeritageAI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeritageAI.API.Controllers
{
    [ApiController]
    [Route("api/admin/orders")]
    [Authorize(Roles = "Admin")]
    public class AdminOrderController : BaseController
    {
        private readonly IOrderService _service;

        public AdminOrderController(IOrderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllOrdersAsync();
            return Ok(result);
        }

        [HttpPut("{orderId}/reassign")]
        public async Task<IActionResult> Reassign(int orderId, ReassignOrderDto dto)
        {
            try
            {
                await _service.ReassignOrderAsync(GetCurrentUserId(), orderId, dto.NewCraftsmanId, dto.Reason);
                return Ok(new { message = "Order reassigned." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }
        [HttpPut("{orderId}/accept")]
        public Task<IActionResult> Accept(int orderId, AdminOrderActionDto dto) =>
    Respond(orderId, true, dto);

        [HttpPut("{orderId}/reject")]
        public Task<IActionResult> Reject(int orderId, AdminOrderActionDto dto) =>
            Respond(orderId, false, dto);

        private async Task<IActionResult> Respond(int orderId, bool accept, AdminOrderActionDto dto)
        {
            try
            {
                await _service.AdminRespondAsync(GetCurrentUserId(), orderId, accept, dto.Reason);
                return Ok(new { message = accept ? "Order accepted." : "Order rejected." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }
    }
}