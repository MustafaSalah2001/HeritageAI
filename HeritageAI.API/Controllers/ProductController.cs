using HeritageAI.Application.Dto.Product;
using HeritageAI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HeritageAI.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    [Authorize(Roles = "Craftsman")]
    public class ProductController : BaseController
    {
        private readonly IProductService _service;
        private readonly IProductImageService _imageService;   // ← جديد
        private readonly IOrderService _orderService;   // ← ضيفها كـ Dependency جديد

        public ProductController(IProductService service, IOrderService orderService, IProductImageService imageService)   // ← جديد
        {
            _service = service;
            _imageService = imageService;
            _orderService = orderService;// ← جديد
        }

       
        [HttpGet("my-sales")]
        public async Task<IActionResult> GetMySales()
        {
            var craftsmanId = GetCurrentUserId();
            var result = await _orderService.GetMySalesAsync(craftsmanId);
            return Ok(result);
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMyProducts()
        {
            var craftsmanId = GetCurrentUserId();
            var result = await _service.GetMyProductsAsync(craftsmanId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            try
            {
                var craftsmanId = GetCurrentUserId();
                var result = await _service.CreateAsync(craftsmanId, dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateProductDto dto)
        {
            try
            {
                var craftsmanId = GetCurrentUserId();
                var result = await _service.UpdateAsync(craftsmanId, id, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException ex) { return Forbid(); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var craftsmanId = GetCurrentUserId();
                await _service.DeleteAsync(craftsmanId, id);
                return Ok(new { message = "Product deleted." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException ex) { return Forbid(); }
        }

        // ==================== Product Images ====================

        [HttpPost("{productId}/images")]
        public async Task<IActionResult> UploadImage(int productId, IFormFile file)
        {
            try
            {
                var craftsmanId = GetCurrentUserId();

                using var stream = file.OpenReadStream();
                var result = await _imageService.UploadImageAsync(craftsmanId, productId, stream, file.FileName);

                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpDelete("images/{imageId}")]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            try
            {
                var craftsmanId = GetCurrentUserId();
                await _imageService.DeleteImageAsync(craftsmanId, imageId);
                return Ok(new { message = "Image deleted." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpPut("images/{imageId}/set-main")]
        public async Task<IActionResult> SetMainImage(int imageId)
        {
            try
            {
                var craftsmanId = GetCurrentUserId();
                await _imageService.SetMainImageAsync(craftsmanId, imageId);
                return Ok(new { message = "Main image updated." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }
    }
}