using HeritageAI.Application.Dto.Craftsman;
using HeritageAI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HeritageAI.API.Controllers
{
    [ApiController]
    [Route("api/craftsman")]
    [Authorize(Roles = "Craftsman")]
    public class CraftsmanController : BaseController
    {
        private readonly ICraftsmanProfileService _service;

        public CraftsmanController(ICraftsmanProfileService service)
        {
            _service = service;
        }
        [HttpPut("me/skills")]
        public async Task<IActionResult> UpdateSkills(UpdateSkillsDto dto)
        {
            var userId = GetCurrentUserId();
            var result = await _service.UpdateSkillsAsync(userId, dto.Skills);
            return Ok(result);
        }
        

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _service.GetMyProfileAsync(userId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile(UpdateCraftsmanProfileDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _service.UpdateMyProfileAsync(userId, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("me/password")]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _service.UpdatePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword);
                return Ok(new { message = "Password updated." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        }

        [HttpDelete("me")]
        public async Task<IActionResult> DeactivateMyAccount()
        {
            try
            {
                var userId = GetCurrentUserId();
                await _service.DeactivateMyAccountAsync(userId);
                return Ok(new { message = "Account deactivated." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        [HttpPost("me/profile-image")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            try
            {
                var userId = GetCurrentUserId();
                using var stream = file.OpenReadStream();
                var url = await _service.UploadProfileImageAsync(userId, stream, file.FileName);
                return Ok(new { profileImageUrl = url });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        [HttpDelete("me/profile-image")]
        public async Task<IActionResult> DeleteProfileImage()
        {
            try
            {
                var userId = GetCurrentUserId();
                await _service.DeleteProfileImageAsync(userId);
                return Ok(new { message = "Profile image deleted." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }
    }
}
