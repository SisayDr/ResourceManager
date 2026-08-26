using Microsoft.Extensions.DependencyInjection;
using ResourceManagerAPI.Data;
using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Models;
using ResourceManagerAPI.Tests.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace ResourceManagerAPI.Tests.Integrations
{
    public class ResourceTypeRoutesTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
    {
        [Fact]
        public async Task GetAll_WithoutAuth_ReturnsUnauthorized()
        {
            var response = await Client.GetAsync("/api/resource-types");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Fact]
        public async Task GetAll_AsNonAdmin_ReturnsForbidden()
        {
            ActAs("Resource Manager");

            var response = await Client.GetAsync("/api/resource-types");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        [Fact]
        public async Task GetAll_AsAdmin_ReturnsOk()
        {
            await SeedResourceTypeAsync("Class-Room");

            ActAs("Admin");

            var response = await Client.GetAsync("/api/resource-types");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var resourceTypes = await response.Content
                .ReadFromJsonAsync<List<ResourceTypeDto>>();

            Assert.NotNull(resourceTypes);
            Assert.Contains(resourceTypes, x => x.Name == "Class-Room");
        }
        [Fact]
        public async Task Create_AsAdmin_ReturnsCreatedResourceType()
        {
            ActAs("Admin");

            var response = await Client.PostAsJsonAsync("/api/resource-types", new ResourceTypeDto("Exam-Room"));
            var body = await response.Content.ReadFromJsonAsync<ResourceType>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Exam-Room", body!.Name);
        }
        [Fact]
        public async Task Create_WithDuplicateName_ReturnsConflict()
        {
            await SeedResourceTypeAsync("Class-Room");
            ActAs("Admin");

            var response = await Client.PostAsJsonAsync("/api/resource-types", new ResourceTypeDto("Class-Room"));

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Delete_WhenNotInUse_ReturnsNoContent()
        {
            var resourceType = await SeedResourceTypeAsync("Exam-Room");
            ActAs("Admin");

            var response = await Client.DeleteAsync($"/api/resource-types/{resourceType.Id}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Delete_WhenInUseByAResource_ReturnsConflict()
        {
            var resourceType = await SeedResourceTypeInUseAsync("Exam-Room");
            ActAs("Admin");

            var response = await Client.DeleteAsync($"/api/resource-types/{resourceType.Id}");

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }


        private async Task<ResourceType> SeedResourceTypeAsync(string name)
        {
            using var scope = GetTestScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var resourceType = new ResourceType { Name = name };
            db.ResourceTypes.Add(resourceType);
            await db.SaveChangesAsync();

            return resourceType;
        }
        private async Task<ResourceType> SeedResourceTypeInUseAsync(string name)
        {
            using var scope = GetTestScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var resourceType = new ResourceType { Name = name };
            var group = new Group { Name = $"Group-{name}" };
            var resource = new Resource
            {
                Name = $"{name} #1",
                TotalCapacity = 1,
                ReservationMode = ReservationMode.shared,
                ResourceTypeId = resourceType.Id,
                GroupId = group.Id,
            };

            db.ResourceTypes.Add(resourceType);
            db.Groups.Add(group);
            db.Resources.Add(resource);
            await db.SaveChangesAsync();

            return resourceType;
        }
    }
}
