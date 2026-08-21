using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Services.Interfaces;

namespace ResourceManagerAPI.Routes
{
    public static class ResourceTypeRoutes
    {
        public static void MapResourceTypeRoutes(this WebApplication app)
        {
            var router = app.MapGroup("/api/resource-types").RequireAuthorization("AdminOnly");

            router.MapGet("/", GetAllResourceTypes);
            router.MapGet("/{id}", GetResourceTypeById);
            router.MapPost("/", CreateResourceType);
            router.MapPut("/{id}", UpdateResourceType);
            router.MapDelete("/{id}", DeleteResourceType);
        }

        public static async Task<IResult> GetAllResourceTypes(IResourceTypeService service)
        {
            var result = await service.GetAllResourceTypes();

            return Results.Ok(result);
        }
        public static async Task<IResult> GetResourceTypeById(Guid id, IResourceTypeService service)
        {
            var result = await service.GetResourceTypebyId(id);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        }

        public static async Task<IResult> CreateResourceType(ResourceTypeDto newResourceType,  IResourceTypeService service)
        {
            var result = await service.CreateResourceType(newResourceType);
            
            return result is not null ? Results.Ok(result) : Results.Conflict(new {message = "Resource Type already exits."});
        }
        public static async Task<IResult> UpdateResourceType(Guid id, ResourceTypeDto updatedResourceType, IResourceTypeService service)
        {
            var result = await service.UpdateResourceType(id, updatedResourceType);

            return result is not null ? Results.Ok(result) : Results.NotFound();
        }

        public static async Task<IResult> DeleteResourceType(Guid id, IResourceTypeService service) { 
            var result = await service.DeleteResourceType(id);

            return result switch
            {
                DbOperationResult.Deleted => Results.NoContent(),
                DbOperationResult.NotFound => Results.NotFound(),
                DbOperationResult.InUse => Results.Conflict(),
                _ => Results.Problem()
            };

        }
    }
}
