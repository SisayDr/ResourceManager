using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ResourceManagerAPI.Data;
using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Models;

namespace ResourceManagerAPI.Services
{
    public class UserService (AppDbContext db, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        public async Task<List<UserResponse>> GetAllUsers()
        {
            var users = await db.Users.ToArrayAsync();
            var result = new List<UserResponse>();

            foreach (var user in users) { 
                result.Add(await ToUserResponse(user));
            }

            return result;
        }

        public async Task<UserResponse?> GetUserById(string id)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user is null) return null;
            return await ToUserResponse(user);
        }
        public async Task<IdentityResult> CreateUser(CreateUserRequest request)
        {
            var newUser = new User { UserName = request.Email, Email = request.Email, FullName = request.FullName};

            var result = await userManager.CreateAsync(newUser, request.Password);
            if (!result.Succeeded) { return result; }

            return await userManager.AddToRoleAsync(newUser, request.Role);
        }

        public async Task<User?> UpdateUser(string  id, User updatedUser)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) {return null;}

            user.FullName = updatedUser.FullName;
            user.Email = updatedUser.Email;

            return user;
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

        private async Task<UserResponse> ToUserResponse(User user)
        {
            var roles = await userManager.GetRolesAsync(user);
            return new UserResponse(user.Id, user.FullName, user.Email!, roles.FirstOrDefault());
        }

    }
}