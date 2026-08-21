using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Models;

namespace ResourceManagerAPI.Services.Interfaces
{
    public interface ICurrentUserService
    {
        Task<UserResponse?> GetProfileAsync();
        Task<string?> GetIdAsync();
        Task<bool> CanAccessGroupAsync(Guid groupId);
        Task<bool> IsAdminAsync();
        Task<bool> IsCreatorOfAsync<T>(T entity) where T : BaseAuditableEntity;
    }
}