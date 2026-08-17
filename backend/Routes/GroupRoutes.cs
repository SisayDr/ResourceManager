using Microsoft.AspNetCore.Mvc;
using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Models;
using ResourceManagerAPI.Services;

namespace ResourceManagerAPI.Routes
{
    public static class GroupRoutes
    {
        public static void MapGroupRoutes(this WebApplication app)
        {
            app.MapGet("/api/groups/", GetAllGroups).RequireAuthorization();
            var groups = app.MapGroup("/api/groups").RequireAuthorization("AdminOnly");

            //groups.MapGet("/", GetAllGroups);
            groups.MapPost("/", CreateGroup);
            groups.MapPut("/{id}", UpdateGroup);
            groups.MapDelete("/{id}", DeleteGroup);
        }

        public static async Task<IResult> GetAllGroups(GroupService service)
        {
            return Results.Ok(await service.GetAllGroups());
        }
        public static async Task<IResult> CreateGroup(GroupDTO newGroupName, GroupService service)
        {
            var result = await service.CreateGroup(newGroupName);
            return Results.Ok(result);
        }
        public static async Task<IResult> UpdateGroup(Guid id, GroupDTO UpdatedGroupName, GroupService service)
        {
            var result = await service.UpdateGroup(id, UpdatedGroupName);

            return Results.Ok(result);
        }
        public static async Task<IResult> DeleteGroup(Guid id, GroupService service)
        {
            var result = await service.DeleteGroup(id);

            return Results.Ok(result);
        }
    }
}
