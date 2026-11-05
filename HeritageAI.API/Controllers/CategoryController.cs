using HeritageAI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeritageAI.API.Controllers
{
    // قراءة فقط لقائمة التصنيفات، لأي مستخدم مسجّل دخول (حرفي عند إضافة/تعديل
    // منتج، أو أي طرف تاني يحتاج القائمة). الإضافة والحذف تظل حصراً عند الأدمن
    // عبر AdminCategoryController.
    [ApiController]
    [Route("api/categories")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }
    }
}