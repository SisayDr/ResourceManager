using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResourceManagerAPI.Data;
using ResourceManagerAPI.Models;

namespace ResourceManagerAPI.Tests
{
    public static class TestFactory
    {
        public static AppDbContext GetDbContext() {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var httpContextAccessor = new HttpContextAccessor();
            return new AppDbContext(options, httpContextAccessor);
        }
        public static async Task<Reservation> SeedReservation(AppDbContext db)
        {
            var group = new Group { Name = "TestGroup" };
            var user = new User { FullName = "TestUser", GroupId = group.Id };
            var resourceType = new ResourceType { Name = "TestResourceType" };

            var resource = new Resource
            {
                Name = "TestResource",
                TotalCapacity = 20,
                GroupId = group.Id,
                ResourceTypeId = resourceType.Id,
                ReservationMode = ReservationMode.exclusive
            };

            var reservation = new Reservation
            {
                Start = DateTimeOffset.UtcNow.AddHours(1),
                End = DateTimeOffset.UtcNow.AddHours(2),
                BookedCapacity = 10,
                Status = ReservationStatus.Confirmed,
                ResourceId = resource.Id,
                User = user
            };

            db.AddRange(group, resourceType, resource, user, reservation);
            await db.SaveChangesAsync();

            return await db.Reservations
                .Include(r => r.Resource)
                .Include(r => r.User)
                .SingleAsync(r => r.Id == reservation.Id);
        }

    }
}