using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ResourceManagerAPI.Data;
using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Models;

namespace ResourceManagerAPI.Extensions{
    public static class UserExtensions{
        public static async Task<UserResponse> ToUserResponseAsync(this User user, UserManager<User> userManager, AppDbContext db){
            var roles = await userManager.GetRolesAsync(user);
            var userGroup = await db.Groups.Where(g => g.Id == user.GroupId).Select(g => g.Name).FirstOrDefaultAsync();
            return new UserResponse( user.Id, user.FullName, user.Email!, roles.FirstOrDefault(), userGroup, user.GroupId);
        }
    }
}
