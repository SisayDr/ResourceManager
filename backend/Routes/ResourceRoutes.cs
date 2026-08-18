using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Services;

namespace ResourceManagerAPI.Routes
{
    public static class ResourceRoutes
    {
        public static void MapResourceRoutes(this WebApplication app) {
            var resources = app.MapGroup("/api/resources").RequireAuthorization();

            resources.MapGet("/", GetAllResources);
            resources.MapGet("/{id}", GetResourceById);
            resources.MapPost("/", CreateResource);
            resources.MapPut("/{id}", UpdateResource);
            resources.MapDelete("/{id}", DeleteResource);
        }

        public static async Task<IResult> GetAllResources(ResourceService service)
        {
            var result = await service.GetAllResources();

            return Results.Ok(result);
        }
        public static async Task<IResult> GetResourceById(Guid id, ResourceService service)
        {
            var result = await service.GetResourcebyId(id);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        }

        public static async Task<IResult> CreateResource(ResourceDto newResource, ResourceService service)
        {
            var result = await service.CreateResource(newResource);

            return Results.Ok(result);
        }
        public static async Task<IResult> UpdateResource(Guid id, ResourceDto updatedResource, ResourceService service)
        {
            var result = await service.UpdateResource(id, updatedResource);

            return result is not null ? Results.Ok(result) : Results.NotFound();
        }

        public static async Task<IResult> DeleteResource(Guid id, ResourceService service)
        {
            var result = await service.DeleteResource(id);

            return result switch
            {
                ResourceService.DbOperationResult.Deleted => Results.NoContent(),
                ResourceService.DbOperationResult.NotFound => Results.NotFound(),
                ResourceService.DbOperationResult.InUse => Results.Conflict(),
                _ => Results.Problem()
            };

        }
    }
}
