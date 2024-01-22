

using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;
            // If the user is authenticated, get the username from the HttpContext
            var userId = resultContext.HttpContext.User.GetUserId();
            // Get the IUserRepository service from the request services
            var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            var user = await repo.GetUserByIdAsync(userId);
            // Update the LastActive property of the user to the current UTC time
            user.LastActive = DateTime.UtcNow;
            // Save the changes to the repository
            await repo.SaveAllAsync();

        }
    }
}