using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Models;

namespace ResourceManagerAPI.Services.Interfaces
{
    public interface IResourceService
    {
        Task<List<Resource>> GetAllResources();
        Task<Resource?> GetResourcebyId(Guid id);
        Task<ResourceResponse?> CreateResource(ResourceRequest request);
        Task<Resource?> UpdateResource(Guid id, ResourceRequest updatedResource);
        Task<DbOperationResult> DeleteResource(Guid id);
    }
}
