using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Services;

namespace ResourceManagerAPI.Routes
{
    public static class UserRoutes
    {
        public static void MapUserRoutes(this WebApplication app)
        {
            app.MapPost("/api/login", Login);
            var users = app.MapGroup("/api/users").RequireAuthorization();

            users.MapGet("/", GetUsers);
            users.MapGet("/{id}", GetUserById);
            users.MapPut("/{id}", UpdateUser);
            users.MapGet("/me", GetCurrentUser);
            users.MapPost("/", CreateUser).RequireAuthorization("AdminOnly");
            users.MapDelete("/{id}", DeleteUser).RequireAuthorization("AdminOnly");
            users.MapPost("/logout", Logout);
        }

        private static async Task<IResult> GetUsers(UserService service)
        {
            var users = await service.GetAllUsers();

            return Results.Ok(users);
        }
        private static async Task<IResult> GetUserById(string id, UserService service)
        {
            var user = await service.GetUserById(id);

            return Results.Ok(user);
        }
        private static async Task<IResult> GetCurrentUser(UserService service)
        {
            var currentUser = await service.GetCurrentUser();
            return Results.Ok(currentUser);
        }
        private static async Task<IResult> CreateUser(UserRequest request, UserService service)
        {
            var result = await service.CreateUser(request);

            if (!result.Succeeded) { return Results.BadRequest(result.Errors); }

            return Results.Created();
        }
        private static async Task<IResult> UpdateUser(string id, UserUpdateRequest request, UserService service)
        {
            var result = await service.UpdateUser(id, request);
            return result is not null ? Results.Ok(result) : Results.BadRequest();

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
