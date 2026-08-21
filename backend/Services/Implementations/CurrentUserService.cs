using Microsoft.AspNetCore.Identity;
using ResourceManagerAPI.Data;
using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Extensions;
using ResourceManagerAPI.Models;
using ResourceManagerAPI.Services.Interfaces;

namespace ResourceManagerAPI.Services.Implementations
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, AppDbContext db) : ICurrentUserService
    {
        public async Task<UserResponse?> GetProfileAsync()
        {
            var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext!.User);
            if (user is null) return null;
            return await user.ToUserResponseAsync(userManager, db);
        }
        public async Task<string?> GetIdAsync()
        {
            var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext!.User);
            return user?.Id;
        }
        public async Task<bool> CanAccessGroupAsync(Guid groupId)
        {
            var user = await GetProfileAsync();
            return user?.Role == "Admin" || user?.GroupId == groupId;
        }
        public async Task<bool> IsAdminAsync()
        {
            var user = await GetProfileAsync();
            return user?.Role == "Admin";
        }
        public async Task<bool> IsCreatorOfAsync<T>(T entity) where T : BaseAuditableEntity
        {
            var userId = await GetIdAsync();
            return userId is not null && entity.CreatedBy == userId;
        }
    }
}