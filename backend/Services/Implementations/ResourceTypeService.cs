using Microsoft.EntityFrameworkCore;
using ResourceManagerAPI.Data;
using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Models;
using ResourceManagerAPI.Services.Interfaces;

namespace ResourceManagerAPI.Services.Implementations
{
    public class ResourceTypeService(AppDbContext db) : IResourceTypeService
    {
        public async Task<List<ResourceType>> GetAllResourceTypes() {
            var resourceTypes = await db.ResourceTypes.ToListAsync();
            return resourceTypes;
        }

        public async Task<ResourceType?> GetResourceTypebyId(Guid id)
        {
            var resourceType = await db.ResourceTypes.FindAsync(id);
            return resourceType;
        }

        public async Task<ResourceType?> CreateResourceType(ResourceTypeDto newResourceType) {
            var alreadyExists = await db.ResourceTypes.AnyAsync(g => g.Name == newResourceType.Name);
            if (alreadyExists) return null;

            var resourceType = new ResourceType { Name = newResourceType.Name };
            await db.ResourceTypes.AddAsync(resourceType);
            await db.SaveChangesAsync();

            return resourceType;
        }

        public async Task<ResourceType?> UpdateResourceType(Guid id, ResourceTypeDto updatedResourceType)
        {
            var resourceType = await db.ResourceTypes.FindAsync(id);
            if (resourceType is null) return null;

            resourceType.Name = updatedResourceType.Name;
            await db.SaveChangesAsync();

            return resourceType;
        }

        public async Task<DbOperationResult> DeleteResourceType(Guid id)
        {
            var resourceType = await db.ResourceTypes.FindAsync(id);
            if(resourceType is null) return DbOperationResult.NotFound;

            var isInUse = await db.Resources.AnyAsync(r => r.ResourceTypeId == id);
            if (isInUse) return DbOperationResult.InUse;

            db.ResourceTypes.Remove(resourceType);
            await db.SaveChangesAsync();

            return DbOperationResult.Deleted;
        }
    }

}
