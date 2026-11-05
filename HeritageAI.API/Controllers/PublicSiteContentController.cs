using HeritageAI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HeritageAI.API.Controllers
{
    [ApiController]
    [Route("api/public/site-content")]
    public class PublicSiteContentController : ControllerBase
    {
        private readonly ISiteContentService _service;

        public PublicSiteContentController(ISiteContentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _service.GetAsync();
            return Ok(result);
        }
    }
}
