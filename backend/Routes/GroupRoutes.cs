using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Services;

namespace ResourceManagerAPI.Routes
{
    public static class GroupRoutes
    {
        public static void MapGroupRoutes(this WebApplication app)
        {
            app.MapGet("/api/groups/", GetAllGroups).RequireAuthorization();
            var router = app.MapGroup("/api/groups").RequireAuthorization("AdminOnly");

            //groups.MapGet("/", GetAllGroups);
            router.MapGet("/{id}", GetGroupById);
            router.MapPost("/", CreateGroup);
            router.MapPut("/{id}", UpdateGroup);
            router.MapDelete("/{id}", DeleteGroup);
        }

        public static async Task<IResult> GetAllGroups(GroupService service)
        {
            return Results.Ok(await service.GetAllGroups());
        }
        public static async Task<IResult> GetGroupById(Guid id, GroupService service)
        {
            var group = await service.GetGroupById(id);
            return group is not null ? Results.Ok(group) : Results.NotFound();
        }
        public static async Task<IResult> CreateGroup(GroupDto newGroup, GroupService service)
        {
            var result = await service.CreateGroup(newGroup);
            return result is not null ? Results.Ok(result) : Results.Conflict(new {message = "Group already exits."});
        }
        public static async Task<IResult> UpdateGroup(Guid id, GroupDto UpdatedGroup, GroupService service)
        {
            var result = await service.UpdateGroup(id, UpdatedGroup);

            return Results.Ok(result);
        }
        public static async Task<IResult> DeleteGroup(Guid id, GroupService service)
        {
            var result = await service.DeleteGroup(id);

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
