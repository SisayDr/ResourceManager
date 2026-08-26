using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ResourceManagerAPI.Data;
using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Models;
using ResourceManagerAPI.Tests.Helpers;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ResourceManagerAPI.Tests.Integration
{
    public class ReservationRoutesTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } };

        [Fact]
        public async Task Create_WithStartInPast_ReturnsBadRequest()
        {
            var (_, resource) = await SeedGroupAndResourceAsync(totalCapacity: 26);
            ActAs("Admin");

            var request = CreateRequestFor(resource.Id, 13, -1);

            var response = await Client.PostAsJsonAsync("/api/reservations", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        [Fact]
        public async Task Create_AsGroupMember_ReturnsConfirmedReservation()
        {
            var (group, resource) = await SeedGroupAndResourceAsync(totalCapacity: 5);
            var user = await SeedUserAsync("BezaBa@ethiopiantest.com", "Abcd@1234", "Resource Manager", group.Id);
            ActAs("Resource Manager", user.Id);

            var request = CreateRequestFor(resource.Id, capacity: 1);

            var response = await Client.PostAsJsonAsync("/api/reservations", request);
            var body = await response.Content.ReadFromJsonAsync<ReservationResponse>(JsonOptions);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(ReservationStatus.Confirmed, body!.Status);
        }
        
        [Fact]
        public async Task Create_AsOutsider_ReturnsPendingReservation()
        {
            var (_, resource) = await SeedGroupAndResourceAsync(totalCapacity: 26);
            var (outsiderGroup, _) = await SeedGroupAndResourceAsync(totalCapacity: 25);
            var outsider = await SeedUserAsync("ElleniG@ethiopiantest.com","Abcd@1234", "Resource Manager", outsiderGroup.Id);
            ActAs("Resource Manager", outsider.Id);

            var request = CreateRequestFor(resource.Id, capacity: 1);

            var response = await Client.PostAsJsonAsync("/api/reservations", request);
            var body = await response.Content.ReadFromJsonAsync<ReservationResponse>(JsonOptions);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(ReservationStatus.Pending, body!.Status);
        }

        [Fact]
        public async Task Create_WhenCapacityAlreadyFullyBooked_ReturnsBadRequest()
        {
            var (group, resource) = await SeedGroupAndResourceAsync(totalCapacity: 26);
            var admin = await SeedUserAsync("SisayDr@ethiopiantest.com", "Abcd@1234", "Admin", group.Id);
            ActAs("Admin", admin.Id);

            var firstRequest = CreateRequestFor(resource.Id, capacity: 13);
            var firstResponse = await Client.PostAsJsonAsync("/api/reservations", firstRequest);
            Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

            // Same time window, same resource - no capacity left for a second booking.
            var secondRequest = CreateRequestFor(resource.Id, capacity: 15);
            var secondResponse = await Client.PostAsJsonAsync("/api/reservations", secondRequest);

            Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
        }

        [Fact]
        public async Task Delete_AsNonCreator_ReturnsUnauthorized()
        {
            var (group1, resource1) = await SeedGroupAndResourceAsync(totalCapacity: 26);
            var (group2, resource2) = await SeedGroupAndResourceAsync(totalCapacity: 26);
            var creator = await SeedUserAsync("BezaBa@ethiopiantest.com", "Abcd@1234", "Resource Manager", group1.Id);
            var someoneElse = await SeedUserAsync("ElleniG@ethiopiantest.com", "Abcd@1234", "Resource Manager", group2.Id);

            ActAs("Resource Manager", creator.Id);
            var createResponse = await Client.PostAsJsonAsync("/api/reservations", CreateRequestFor(resource1.Id, capacity: 13));
            var created = await createResponse.Content.ReadFromJsonAsync<ReservationResponse>(JsonOptions);

            ActAs("Resource Manager", someoneElse.Id);
            var deleteResponse = await Client.DeleteAsync($"/api/reservations/{created!.Id}");

            Assert.Equal(HttpStatusCode.Unauthorized, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task Delete_AsCreator_ReturnsNoContent()
        {
            var (group, resource) = await SeedGroupAndResourceAsync(totalCapacity: 26);
            var creator = await SeedUserAsync("BezaBa@ethiopiantest.com", "Abcd@1234", "Resource Manager", group.Id);

            ActAs("Resource Manager", creator.Id);
            var createResponse = await Client.PostAsJsonAsync("/api/reservations", CreateRequestFor(resource.Id, capacity: 13));
            var created = await createResponse.Content.ReadFromJsonAsync<ReservationResponse>(JsonOptions);

            var deleteResponse = await Client.DeleteAsync($"/api/reservations/{created!.Id}");

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }


        private static ReservationRequest CreateRequestFor(Guid resourceId, int capacity, int daysFromNow = 0)
        {
            return new(
                Start: DateTimeOffset.UtcNow.AddDays(daysFromNow).AddHours(1),
                End: DateTimeOffset.UtcNow.AddDays(daysFromNow).AddHours(2),
                BookedCapacity: capacity,
                Status: null,
                ResourceId: resourceId
            );
        }
        private async Task<(Group group, Resource resource)> SeedGroupAndResourceAsync(int totalCapacity)
        {
            using var scope = GetTestScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var group = new Group { Name = $"Group {Guid.NewGuid()}" };
            var resourceType = new ResourceType { Name = $"Type {Guid.NewGuid()}" };
            var resource = new Resource
            {
                Name = $"Resource-{Guid.NewGuid()}",
                TotalCapacity = totalCapacity,
                ReservationMode = ReservationMode.shared,
                GroupId = group.Id,
                ResourceTypeId = resourceType.Id
            };

            db.Groups.Add(group);
            db.ResourceTypes.Add(resourceType);
            db.Resources.Add(resource);
            await db.SaveChangesAsync();

            return (group, resource);
        }

        private async Task<User> SeedUserAsync(string email, string password, string role, Guid groupId)
        {
            using var scope = GetTestScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var user = new User { UserName = email, Email = email, FullName = "Test User", GroupId = groupId };
            await userManager.CreateAsync(user, password);
            await userManager.AddToRoleAsync(user, role);

            return user;
        }
    }
}
