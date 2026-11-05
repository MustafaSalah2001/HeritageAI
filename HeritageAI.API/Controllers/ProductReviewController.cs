using HeritageAI.Application.Dto.Review;
using HeritageAI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeritageAI.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class ProductReviewController : BaseController
    {
        private readonly IProductReviewService _service;
        public ProductReviewController(IProductReviewService service) => _service = service;

        [HttpGet("public/products/{productId:int}/reviews")]
        public async Task<IActionResult> GetForProduct(int productId) =>
            Ok(await _service.GetForProductAsync(productId));

        [Authorize(Roles = "Buyer")]
        [HttpPost("reviews")]
        public async Task<IActionResult> Create(CreateReviewDto dto)
        {
            try
            {
                var result = await _service.CreateAsync(GetCurrentUserId(), dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Buyer")]
        [HttpGet("reviews/mine")]
        public async Task<IActionResult> GetMine() =>
            Ok(await _service.GetMineAsync(GetCurrentUserId()));
    }
}