using Microsoft.EntityFrameworkCore;
using ResourceManagerAPI.Data;
using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Models;

namespace ResourceManagerAPI.Services
{
    public class ResourceService(AppDbContext db)
    {
        public async Task<List<Resource>> GetAllResources()
        {
            var resources = await db.Resources.ToListAsync();
            return resources;
        }

        public async Task<Resource?> GetResourcebyId(Guid id)
        {
            var resource = await db.Resources.FindAsync(id);
            return resource;
        }

        public async Task<Resource> CreateResource(ResourceDto newResource)
        {
            var resource = new Resource{
                Name = newResource.Name,
                TotalCapacity = newResource.TotalCapacity,
                ReservationMode = (Models.ReservationMode) newResource.ReservationMode,
                ResourceTypeId = newResource.ResourceTypeId,
                GroupId = newResource.GroupId
            };
            await db.Resources.AddAsync(resource);
            await db.SaveChangesAsync();

            return resource;
        }

        public async Task<Resource?> UpdateResource(Guid id, ResourceDto updatedResource)
        {
            var resource = await db.Resources.FindAsync(id);
            if (resource is null) return null;

            resource.Name = updatedResource.Name;
            resource.TotalCapacity = updatedResource.TotalCapacity;
            resource.ReservationMode = (Models.ReservationMode) updatedResource.ReservationMode;
            resource.ResourceTypeId = updatedResource.ResourceTypeId;
            resource.GroupId = updatedResource.GroupId;

            await db.SaveChangesAsync();
            return resource;
        }

        public async Task<DbOperationResult> DeleteResource(Guid id)
        {
            var resource = await db.Resources.FindAsync(id);
            if (resource is null) return DbOperationResult.NotFound;

            var isInUse = await db.Reservations.AnyAsync(r => r.ResourceId == id);
            if (isInUse) return DbOperationResult.InUse;

            db.Resources.Remove(resource);
            await db.SaveChangesAsync();

            return DbOperationResult.Deleted;
        }
        public enum DbOperationResult { Deleted, NotFound, InUse }
    }
}
