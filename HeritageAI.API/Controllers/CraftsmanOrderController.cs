using HeritageAI.Application.Dto.Order;
using HeritageAI.Application.Interfaces;
using HeritageAI.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeritageAI.API.Controllers
{
    // ملاحظة: مبيعات الحرفي (GetMySales) موجودة سابقًا تحت
    // api/products/my-sales. هاد الكنترولر بضيف فقط عملية تحديث
    // حالة الطلب، تحت مسار منطقي أوضح: api/craftsman/orders.
    [ApiController]
    [Route("api/craftsman/orders")]
    [Authorize(Roles = "Craftsman")]
    public class CraftsmanOrderController : BaseController
    {
        private readonly IOrderService _service;

        public CraftsmanOrderController(IOrderService service)
        {
            _service = service;
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusDto dto)
        {
            if (!Enum.TryParse<OrderStatus>(dto.Status, ignoreCase: true, out var newStatus))
            {
                return BadRequest(new
                {
                    message = $"Invalid status '{dto.Status}'. Valid values: " +
                               string.Join(", ", Enum.GetNames(typeof(OrderStatus)))
                });
            }

            try
            {
                var craftsmanId = GetCurrentUserId();
                var result = await _service.CraftsmanUpdateStatusAsync(craftsmanId, id, newStatus, dto.Reason);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }
    }
}