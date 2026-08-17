using Microsoft.AspNetCore.Identity;

namespace ResourceManagerAPI.Services
{
    public static class ServicesHanlder
    {
        public static void AddServices(this WebApplicationBuilder builder) {
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<GroupService>();
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
