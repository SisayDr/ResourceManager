using Microsoft.AspNetCore.Identity;
using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Extensions;
using ResourceManagerAPI.Models;

namespace ResourceManagerAPI.Services
{
    public class UserAccessor(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
    {
        public async Task<UserResponse?> GetCurrentUserAsync()
        {
            var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext!.User);
            if (user is null) return null;
            return await user.ToUserResponseAsync(userManager);
        }
        public async Task<string?> GetCurrentUserIdAsync()
        {
            var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext!.User);
            return user?.Id;
        }
    }
}
