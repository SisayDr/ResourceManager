using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Services.Interfaces;

namespace ResourceManagerAPI.Routes
{
    public static class ResourceRoutes
    {
        public static void MapResourceRoutes(this WebApplication app) {
            var router = app.MapGroup("/api/resources").RequireAuthorization();

            router.MapGet("/", GetAllResources);
            router.MapGet("/{id}", GetResourceById);
            router.MapPost("/", CreateResource);
            router.MapPut("/{id}", UpdateResource);
            router.MapDelete("/{id}", DeleteResource);
        }

        public static async Task<IResult> GetAllResources(IResourceService service)
        {
            var result = await service.GetAllResources();

            return Results.Ok(result);
        }
        public static async Task<IResult> GetResourceById(Guid id, IResourceService service)
        {
            var result = await service.GetResourcebyId(id);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        }

        public static async Task<IResult> CreateResource(ResourceRequest newResource, IResourceService service)
        {
            var result = await service.CreateResource(newResource);

            return result is not null ? Results.Ok(result) : Results.BadRequest();
        }
        public static async Task<IResult> UpdateResource(Guid id, ResourceRequest updatedResource, IResourceService service)
        {
            var result = await service.UpdateResource(id, updatedResource);

            return result is not null ? Results.Ok(result) : Results.NotFound();
        }

        public static async Task<IResult> DeleteResource(Guid id, IResourceService service)
        {
            var result = await service.DeleteResource(id);

            return result switch
            {
                DbOperationResult.Deleted => Results.NoContent(),
                DbOperationResult.NotFound => Results.NotFound(),
                DbOperationResult.InUse => Results.Conflict(),
                DbOperationResult.UnAuthorized => Results.Unauthorized(),
                _ => Results.Problem()
            };

        }
    }
}
