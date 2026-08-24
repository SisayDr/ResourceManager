using Moq;
using ResourceManagerAPI.Data;
using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Models;
using ResourceManagerAPI.Services.Implementations;
using ResourceManagerAPI.Services.Interfaces;

namespace ResourceManagerAPI.Tests.Units
{
    public class ReservationServiceTests
    {
        private readonly AppDbContext _db;
        private readonly Mock<ICurrentUserService> _currentUser;
        private readonly ReservationService _service;

        public ReservationServiceTests()
        {
            _db = TestFactory.GetDbContext();
            _currentUser = new Mock<ICurrentUserService>();
            _service = new ReservationService(_db, _currentUser.Object);
        }

        [Fact]
        public async Task GetAllReservations_ReturnsReservations_WhenUserHasAccess()
        {
            //Arrange
            var reservation1 = await TestFactory.SeedReservation(_db);
            var reservation2 = await TestFactory.SeedReservation(_db); 
            var reservation3 = await TestFactory.SeedReservation(_db);

            _currentUser.Setup(c => c.CanAccessGroupAsync(reservation1.Resource.GroupId)).ReturnsAsync(true);
            _currentUser.Setup(c => c.CanAccessGroupAsync(reservation2.Resource.GroupId)).ReturnsAsync(true);
            _currentUser.Setup(c => c.CanAccessGroupAsync(reservation3.Resource.GroupId)).ReturnsAsync(false);

            //Act
            var result = await _service.GetAllReservations();

            //Assert
            Assert.Equal(2, result.Count);
            Assert.DoesNotContain(result, r => r.Id  == reservation3.Id);
        }

        [Fact]
        public async Task GetReservationById_ReturnsReservation_WhenUserHasAccess()
        {
            var reservation = await TestFactory.SeedReservation(_db);
            _currentUser.Setup(c => c.CanAccessGroupAsync(reservation.Resource.GroupId)).ReturnsAsync(true);

            var result = await _service.GetReservationById(reservation.Id);

            Assert.NotNull(result);
            Assert.Equal(reservation.Id, result.Id);
        }


        [Fact]
        public async Task CreateReservation_ReturnsNull_WhenStartIsInThePast()
        {
            var reservation = await TestFactory.SeedReservation(_db);
            var request = new ReservationRequest(DateTimeOffset.UtcNow.AddHours(-1),DateTimeOffset.UtcNow.AddHours(2), 10, null, reservation.ResourceId);
         
            var result = await _service.CreateReservation(request);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateReservation_ReturnsNull_WhenCapacityExceeds()
        {
            var reservation = await TestFactory.SeedReservation(_db);
            var request = new ReservationRequest(DateTimeOffset.UtcNow.AddHours(1), DateTimeOffset.UtcNow.AddHours(2), 12, null, reservation.ResourceId);

            var result = await _service.CreateReservation(request);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateReservation_ReturnsConfirmedReservation_WhenUserHasAccessToGroup()
        {
            var reservation = await TestFactory.SeedReservation(_db);
            var request = new ReservationRequest(DateTimeOffset.UtcNow.AddHours(1), DateTimeOffset.UtcNow.AddHours(2), 10, null, reservation.ResourceId);
            _currentUser.Setup(c => c.CanAccessGroupAsync(reservation.Resource.GroupId)).ReturnsAsync(true);

            var result = await _service.CreateReservation(request);

            Assert.NotNull(result);
            Assert.Equal(ReservationStatus.Confirmed, result.Status);
        }

        [Fact]
        public async Task CreateReservation_ReturnsPendingReservation_WhenUserHasNoAccessToGroup()
        {
            var reservation = await TestFactory.SeedReservation(_db);
            var request = new ReservationRequest(DateTimeOffset.UtcNow.AddHours(1), DateTimeOffset.UtcNow.AddHours(2), 10, null, reservation.ResourceId);

            var result = await _service.CreateReservation(request);

            Assert.NotNull(result);
            Assert.Equal(ReservationStatus.Pending, result.Status);
        }

        [Fact]
        public async Task UpdateReservation_ReturnsUpdatedReservationExceptStatus_WhenUserIsCreator()
        {
            var reservation = await TestFactory.SeedReservation(_db);
            var request = new ReservationRequest(DateTimeOffset.UtcNow.AddHours(1), DateTimeOffset.UtcNow.AddHours(2), 5, ReservationStatus.Canceled, reservation.ResourceId);
            _currentUser.Setup(c => c.IsCreatorOfAsync(reservation)).ReturnsAsync(true);
            _currentUser.Setup(c => c.CanAccessGroupAsync(reservation.Resource.GroupId)).ReturnsAsync(false);

            var result = await _service.UpdateReservation(reservation.Id, request);

            Assert.NotNull(result);
            Assert.Equal(5, result.BookedCapacity);
            Assert.Equal(reservation.Status, result.Status);
        }

        [Fact]
        public async Task DeleteReservation_ReturnsNotFound_WhenReservationDoesNotExist()
        {
            var id = Guid.NewGuid();

            var result = await _service.DeleteReservation(id);

            Assert.Equal(DbOperationResult.NotFound, result);
        }

        [Fact]
        public async Task DeleteReservation_ReturnsUnAuthorized_WhenUserIsNotTheCreator()
        {
            var reservation = await TestFactory.SeedReservation(_db);
            _currentUser.Setup(c => c.IsCreatorOfAsync(reservation)).ReturnsAsync(false);

            var result = await _service.DeleteReservation(reservation.Id);

            Assert.Equal(DbOperationResult.UnAuthorized, result);
        }

        [Fact]
        public async Task DeleteReservation_ReturnsDeleted_WhenUserIsTheCreator()
        {
            var reservation = await TestFactory.SeedReservation(_db);
            _currentUser.Setup(c => c.IsCreatorOfAsync(reservation)).ReturnsAsync(true);

            var result = await _service.DeleteReservation(reservation.Id);

            Assert.Equal(DbOperationResult.Deleted, result);
        }

    }
}
