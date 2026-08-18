using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ResourceManagerAPI.Data;
using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Extensions;
using ResourceManagerAPI.Models;

namespace ResourceManagerAPI.Services
{
    public class UserService (AppDbContext db, UserManager<User> userManager, SignInManager<User> signInManager, UserAccessor userAccessor)
    {
        public async Task<List<UserResponse>> GetAllUsers()
        {
            var users = await db.Users.ToArrayAsync();
            var result = new List<UserResponse>();

            foreach (var user in users) { 
                result.Add(await user.ToUserResponseAsync(userManager));
            }

            return result;
        }

        public async Task<UserResponse?> GetUserById(string id)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user is null) return null;
            return await user.ToUserResponseAsync(userManager);
        }
        public async Task<IdentityResult> CreateUser(UserRequest request)
        {
            var newUser = new User { UserName = request.Email, Email = request.Email, FullName = request.FullName};

            var result = await userManager.CreateAsync(newUser, request.Password);
            if (!result.Succeeded) { return result; }

            return await userManager.AddToRoleAsync(newUser, request.Role);
        }

        public async Task<IdentityResult?> UpdateUser(string  id, UserUpdateRequest updatedUser)
        {
            var currentUser = await userAccessor.GetCurrentUserAsync();
            var user = await userManager.FindByIdAsync(id);
            if (user == null || currentUser == null) {return null;}

            bool isAdmin = currentUser.Role == "Admin";
            bool isOwnUser = currentUser.Id == id;
            if (!isAdmin && !isOwnUser) return null;

            user.FullName = updatedUser.FullName;
            user.Email = updatedUser.Email;
            user.UserName = updatedUser.Email;

            var result = await userManager.UpdateAsync(user);
            if(!result.Succeeded || !isAdmin) { return result; }

            if(updatedUser.Password is not null)
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                result = await userManager.ResetPasswordAsync(user, token, updatedUser.Password);
            }
            if (updatedUser.Role is not null)
            {
                var currentRole = (await userManager.GetRolesAsync(user)).FirstOrDefault();
                if (currentRole is not null && currentRole != updatedUser.Role)
                {
                    await userManager.RemoveFromRoleAsync(user, currentRole);
                    result = await userManager.AddToRoleAsync(user, updatedUser.Role);
                }
            }
            return result;
        }

        public async Task<bool> DeleteUser(string id) {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if(user == null) {return false;}

            db.Users.Remove(user);
            await db.SaveChangesAsync();

            return true;
        }
        public async Task<SignInResult> Login(string email, string password)
        {
            return await signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: true);
        }
        public async Task Logout() { await signInManager.SignOutAsync(); }

        public async Task<UserResponse?> GetCurrentUser(){return await userAccessor.GetCurrentUserAsync();}
    }
}