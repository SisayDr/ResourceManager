using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Services;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace ResourceManagerAPI.Routes
{
    public static class UserRoutes
    {
        public static void MapUserRoutes(this WebApplication app)
        {
            var users = app.MapGroup("/api/users");

            users.MapGet("/", GetUsers);
            users.MapPost("/", CreateUser);
            users.MapDelete("/{id}", DeleteUser);
            users.MapPost("/login", Login);
            users.MapPost("/logout", Logout).RequireAuthorization();
        }

        private static async Task<IResult> GetUsers(UserService service)
        {
            var users = await service.GetAllUsers();

            return Results.Ok(users);
        }
        private static async Task<IResult> CreateUser(CreateUserRequest request, UserService service)
        {
            var result = await service.CreateUser(request);

            if (!result.Succeeded) { return Results.BadRequest(result.Errors); }

            return Results.Created();
        }
        private static async Task<IResult> DeleteUser(string id, UserService service) { 
            var deleted = await service.DeleteUser(id);

            return deleted ? Results.NoContent() : Results.NotFound();
        }
        private static async Task<IResult> Login(LoginRequest request, UserService service)
        {
            var result = await service.Login(request.Email, request.Password);
            if (!result.Succeeded) return Results.Unauthorized();

            return Results.Ok();
        }
        private static async Task<IResult> Logout(UserService service) {
            await service.Logout();
            return Results.Ok();
        }
    }
}
