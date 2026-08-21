using Microsoft.EntityFrameworkCore;
using ResourceManagerAPI.Data;
using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Extensions;
using ResourceManagerAPI.Models;
using ResourceManagerAPI.Services.Interfaces;

namespace ResourceManagerAPI.Services.Implementations
{
    public class ResourceService(AppDbContext db, ICurrentUserService currentUser) :IResourceService
    {
        public async Task<List<Resource>> GetAllResources()
        {
            var currentUserProfile = await currentUser.GetProfileAsync();
            if (currentUserProfile.Role == "Admin") return await db.Resources.ToListAsync();
            return await db.Resources.Where(r => r.GroupId == currentUserProfile.GroupId).ToListAsync();
        }

        public async Task<Resource?> GetResourcebyId(Guid id)
        {
            var resource = await db.Resources.FindAsync(id);
            if(resource is null) return null;

            return (await currentUser.CanAccessGroupAsync(resource.GroupId)) ? resource : null;
        }

        public async Task<ResourceResponse?> CreateResource(ResourceRequest request)
        {
            Resource newResource = request.ToResource();

            if (!await currentUser.CanAccessGroupAsync(request.GroupId)) return null;

            await db.Resources.AddAsync(newResource);
            await db.SaveChangesAsync();

            return newResource.ToResourceResponse();
        }

        public async Task<Resource?> UpdateResource(Guid id, ResourceRequest updatedResource)
        {
            var isAdmin = await currentUser.IsAdminAsync();
            var resource = await db.Resources.FindAsync(id);

            if (resource is null) return null;
            if(!await currentUser.CanAccessGroupAsync(resource.GroupId)) return null;

            resource.Name = updatedResource.Name;
            resource.TotalCapacity = updatedResource.TotalCapacity;
            resource.ReservationMode = (ReservationMode) updatedResource.ReservationMode;
            resource.ResourceTypeId = updatedResource.ResourceTypeId;
            
            if(isAdmin) resource.GroupId = updatedResource.GroupId;

            await db.SaveChangesAsync();
            return resource;
        }

        public async Task<DbOperationResult> DeleteResource(Guid id)
        {
            var currentUserProfile = await currentUser.GetProfileAsync();
            var resource = await db.Resources.FindAsync(id);
            if (resource is null) return DbOperationResult.NotFound;

            if (!await currentUser.CanAccessGroupAsync(resource.GroupId)) return DbOperationResult.UnAuthorized;

            var isInUse = await db.Reservations.AnyAsync(r => r.ResourceId == id);
            if (isInUse) return DbOperationResult.InUse;

            db.Resources.Remove(resource);
            await db.SaveChangesAsync();

            return DbOperationResult.Deleted;
        }
    }
}
