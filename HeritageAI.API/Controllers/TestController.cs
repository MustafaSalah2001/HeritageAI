using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HeritageAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("public")]
        public IActionResult PublicEndpoint()
        {
            return Ok(new { message = "أي حد يقدر يشوف ده، من غير تسجيل دخول." });
        }

        [Authorize]
        [HttpGet("authenticated-only")]
        public IActionResult AuthenticatedOnly()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                message = "لو شفت ده، يبقى التوكن بتاعك شغال صح.",
                yourEmail = email,
                yourRole = role
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnly()
        {
            return Ok(new { message = "أنت Admin فعلاً، تمام." });
        }
    }
}