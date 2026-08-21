using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Models;

namespace ResourceManagerAPI.Extensions
{
    public static class ResourceExtensions
    {
        public static ResourceResponse ToResourceResponse(this Resource resource)
        {
            return new ResourceResponse(
                resource.Id,
                resource.Name,
                resource.TotalCapacity,
                (ReservationMode)resource.ReservationMode,
                resource.ResourceTypeId,
                resource.GroupId
            );
        }

        public static Resource ToResource(this ResourceRequest request)
        {
            return new Resource
            {
                Name = request.Name,
                TotalCapacity = request.TotalCapacity,
                ReservationMode = (ReservationMode)request.ReservationMode,
                ResourceTypeId = request.ResourceTypeId,
                GroupId = request.GroupId
            };
        }
    }
}