using Microsoft.AspNetCore.Identity;
using ResourceManagerAPI.Services.Implementations;
using ResourceManagerAPI.Services.Interfaces;

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
        }

        public static async Task SeedRoles(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roles = ["Admin", "Resource Manager"];

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role)) await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
