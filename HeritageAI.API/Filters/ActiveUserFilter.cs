using HeritageAI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace HeritageAI.API.Filters
{
    public class ActiveUserFilter : IAsyncActionFilter
    {
        private readonly IUserRepository _userRepository;

        public ActiveUserFilter(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (idClaim is not null && int.TryParse(idClaim, out var userId))
                {
                    var dbUser = await _userRepository.GetByIdAsync(userId);

                    if (dbUser is null || !dbUser.IsActive)
                    {
                        context.Result = new UnauthorizedObjectResult(new { message = "Your account has been deactivated." });
                        return;
                    }
                }
            }

            await next();
        }
    }
}
