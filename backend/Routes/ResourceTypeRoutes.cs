using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Services;

namespace ResourceManagerAPI.Routes
{
    public static class ResourceTypeRoutes
    {
        public static void MapResourceTypeRoutes(this WebApplication app)
        {
            var resourceTypes = app.MapGroup("/api/resource-types").RequireAuthorization("AdminOnly");

            resourceTypes.MapGet("/", GetAllResourceTypes);
            resourceTypes.MapGet("/{id}", GetResourceTypeById);
            resourceTypes.MapPost("/", CreateResourceType);
            resourceTypes.MapPut("/{id}", UpdateResourceType);
            resourceTypes.MapDelete("/{id}", DeleteResourceType);
        }

        public static async Task<IResult> GetAllResourceTypes(ResourceTypeService service)
        {
            var result = await service.GetAllResourceTypes();

            return Results.Ok(result);
        }
        public static async Task<IResult> GetResourceTypeById(Guid id, ResourceTypeService service)
        {
            var result = await service.GetResourceTypebyId(id);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        }

        public static async Task<IResult> CreateResourceType(ResourceTypeDto newResourceType,  ResourceTypeService service)
        {
            var result = await service.CreateResourceType(newResourceType);
            
            return result is not null ? Results.Ok(result) : Results.Conflict(new {message = "Resource Type already exits."});
        }
        public static async Task<IResult> UpdateResourceType(Guid id, ResourceTypeDto updatedResourceType, ResourceTypeService service)
        {
            var result = await service.UpdateResourceType(id, updatedResourceType);

            return result is not null ? Results.Ok(result) : Results.NotFound();
        }

        public static async Task<IResult> DeleteResourceType(Guid id, ResourceTypeService service) { 
            var result = await service.DeleteResourceType(id);

            return result switch
            {
                ResourceTypeService.DbOperationResult.Deleted => Results.NoContent(),
                ResourceTypeService.DbOperationResult.NotFound => Results.NotFound(),
                ResourceTypeService.DbOperationResult.InUse => Results.Conflict(),
                _ => Results.Problem()
            };

        }
    }
}
