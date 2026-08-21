using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Models;

namespace ResourceManagerAPI.Services.Interfaces
{
    public interface IResourceTypeService
    {
        Task<List<ResourceType>> GetAllResourceTypes();
        Task<ResourceType?> GetResourceTypebyId(Guid id);
        Task<ResourceType?> CreateResourceType(ResourceTypeDto newResourceType);
        Task<ResourceType?> UpdateResourceType(Guid id, ResourceTypeDto updatedResourceType);
        Task<DbOperationResult> DeleteResourceType(Guid id);
    }
}
