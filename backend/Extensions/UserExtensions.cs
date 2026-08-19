using Microsoft.AspNetCore.Identity;
using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Models;

namespace ResourceManagerAPI.Extensions{
    public static class UserExtensions{
        public static async Task<UserResponse> ToUserResponseAsync(this User user, UserManager<User> userManager){
            var roles = await userManager.GetRolesAsync(user);
            return new UserResponse( user.Id, user.FullName, user.Email!, roles.FirstOrDefault(), user.GroupId);
        }
    }
}
