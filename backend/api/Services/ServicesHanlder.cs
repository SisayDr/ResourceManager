using Microsoft.AspNetCore.Identity;
using ResourceManagerAPI.Services.Implementations;
using ResourceManagerAPI.Services.Interfaces;
using ResourceManagerAPI.Services.Validators;

namespace ResourceManagerAPI.Services
{
    public static class ServicesHanlder
    {
        public static void AddServices(this WebApplicationBuilder builder) {
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IGroupService, GroupService>();
            builder.Services.AddScoped<IResourceTypeService, ResourceTypeService>();
            builder.Services.AddScoped<IResourceService, ResourceService>();
            builder.Services.AddScoped<IReservationService, ReservationService>();

            builder.Services.AddScoped<IReservationValidator, ReservationTimeValidator>();
            builder.Services.AddScoped<IReservationValidator, ResourceExistsValidator>();
            builder.Services.AddScoped<IReservationValidator, ReservationExistsValidator>();
            builder.Services.AddScoped<IReservationValidator, ReservationCapacityValidator>();
        }

        public static async Task SeedRoles(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roles = ["Admin", "Resource Manager"];

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role)) await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
