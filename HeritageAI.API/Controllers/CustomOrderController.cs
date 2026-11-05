using HeritageAI.Application.Dto.CustomOrder;
using HeritageAI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HeritageAI.API.Controllers
{
    [ApiController]
    [Route("api/custom-orders")]
    [Authorize(Roles = "Buyer")]
    public class CustomOrderController : BaseController
    {
        private readonly ICustomOrderService _service;

        public CustomOrderController(ICustomOrderService service)
        {
            _service = service;
        }

        

        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomOrderDto dto)
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
        public async Task<IActionResult> GetMyRequests()
        {
            var buyerId = GetCurrentUserId();
            var result = await _service.GetMyRequestsAsBuyerAsync(buyerId);
            return Ok(result);
        }
    }
}
