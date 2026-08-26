using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Models;
using ResourceManagerAPI.Tests.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace ResourceManagerAPI.Tests.Integrations
{
    public class AuthTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
    {
        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOk()
        {
            await SeedUserAsync("ElleniG@ethiopiantest.com", "Abcd@1234", "Resource Manager");

            var response = await Client.PostAsJsonAsync("/api/login", new LoginRequest("ElleniG@ethiopiantest.com", "Abcd@1234"));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task Login_WithWrongPassword_ReturnsUnauthorized()
        {
            await SeedUserAsync("BezaBa@ethiopiantest.com", "Abcd@1234", "Resource Manager");

            var response = await Client.PostAsJsonAsync("/api/login", new LoginRequest("BezaBa@ethiopiantest.com", "Abcd1234"));

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetCurrentUser_WithoutAuth_ReturnsUnauthorized()
        {
            var response = await Client.GetAsync("/api/users/me");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Fact]
        public async Task GetCurrentUser_WhenAuthenticated_ReturnsSeededUser()
        {
            var user = await SeedUserAsync("BezaBa@ethiopiantest.com", "Abcd@1234", "Resource Manager");

            ActAs("Resource Manager", user.Id);

            var response = await Client.GetAsync("/api/users/me");
            var body = await response.Content.ReadFromJsonAsync<UserResponse>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(user.Email, body!.Email);
        }


        private async Task<User> SeedUserAsync(string email, string password, string role)
        {
            using var scope = GetTestScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var user = new User { UserName = email, Email = email, FullName = "Test User" };
            await userManager.CreateAsync(user, password);
            await userManager.AddToRoleAsync(user, role);

            return user;
        }
    }
}
