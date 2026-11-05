using HeritageAI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HeritageAI.API.Controllers
{
    [ApiController]
    [Route("api/public/craftsmen")]
    public class PublicCraftsmanController : ControllerBase
    {
        private readonly IPublicCraftsmanService _service;

        public PublicCraftsmanController(IPublicCraftsmanService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllApprovedAsync();
            return Ok(result);
        }
        [HttpGet("{craftsmanId}")]
        public async Task<IActionResult> GetProfile(int craftsmanId)
        {
            try
            {
                var result = await _service.GetPublicProfileAsync(craftsmanId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("{craftsmanId}/products")]
        public async Task<IActionResult> GetProducts(int craftsmanId)
        {
            var result = await _service.GetCraftsmanProductsAsync(craftsmanId);
            return Ok(result);
        }
    }
}
