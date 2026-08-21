using Microsoft.AspNetCore.Identity;
using ResourceManagerAPI.DTOs;

namespace ResourceManagerAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserResponse>> GetAllUsers();
        Task<UserResponse?> GetUserById(string id);
        Task<IdentityResult> CreateUser(UserRequest request);
        Task<IdentityResult?> UpdateUser(string id, UserUpdateRequest updatedUser);
        Task<bool> DeleteUser(string id);
        Task<SignInResult> Login(string email, string password);
        Task Logout();
        Task<UserResponse?> GetCurrentUser();
    }
}
