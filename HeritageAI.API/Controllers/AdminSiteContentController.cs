using HeritageAI.Application.Dto.SiteContent;
using HeritageAI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeritageAI.API.Controllers
{
    [ApiController]
    [Route("api/admin/site-content")]
    [Authorize(Roles = "Admin")]
    public class AdminSiteContentController : ControllerBase
    {
        private readonly ISiteContentService _service;

        public AdminSiteContentController(ISiteContentService service)
        {
            _service = service;
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateSiteContentDto dto)
        {
            var result = await _service.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpPost("images")]
        public async Task<IActionResult> AddImage(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var result = await _service.AddImageAsync(stream, file.FileName);
            return Ok(result);
        }

        [HttpDelete("images/{imageId}")]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            try
            {
                await _service.DeleteImageAsync(imageId);
                return Ok(new { message = "Image deleted." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
