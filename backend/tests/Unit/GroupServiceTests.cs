using Microsoft.EntityFrameworkCore;
using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Data;
using ResourceManagerAPI.Models;
using ResourceManagerAPI.Services.Implementations;
using ResourceManagerAPI.Tests.Helpers;

namespace ResourceManagerAPI.Tests.Unit
{
    public class GroupServiceTests
    {
        private readonly AppDbContext _db;
        private readonly GroupService _service;
        public GroupServiceTests()
        {
            _db = UnitTestsFactory.GetDbContext();
            _service = new GroupService(_db);
        }

        [Fact]
        public async Task GetAllGroups_ReturnsAllGroups()
        {
            _db.Groups.AddRange([new Group { Name = "AMT" }, new Group { Name = "Cabin Crew"}]);
            await _db.SaveChangesAsync();

            var result = await _service.GetAllGroups();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, g => g.Name == "AMT");
            Assert.Contains(result, g => g.Name == "Cabin Crew");
        }

        [Fact]
        public async Task GetGroupById_ReturnsGroup_WhenFound()
        {
            Group newGroup = new Group { Name = "AMT" };
            _db.Groups.Add(newGroup);
            await _db.SaveChangesAsync();

            var result = await _service.GetGroupById(newGroup.Id);

            Assert.NotNull(result);
            Assert.Equal(newGroup.Id, result.Id);
            Assert.Equal(newGroup.Name, result.Name);
        }

        [Fact]
        public async Task GetGroupById_ReturnsNull_WhenNotFound()
        {
            var id = Guid.NewGuid();

            var result = await _service.GetGroupById(id);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateGroup_CreatesGroup_WhenNameDoesNotExist()
        {
            var newGroupRequest = new GroupDto("AMT");

            var result = await _service.CreateGroup(newGroupRequest);

            Assert.NotNull(result);
            Assert.Equal("AMT", result.Name);
            var createdGroup = await _db.Groups.SingleAsync(g => g.Id == result.Id);
            Assert.NotNull(createdGroup);
            Assert.Equal("AMT", createdGroup.Name);
        }

        [Fact]
        public async Task CreateGroup_ReturnsNull_WhenNameAlreadyExist()
        {
            _db.Groups.Add(new Group {Name = "AMT" });
            await _db.SaveChangesAsync();
            var newGroupRequest = new GroupDto("AMT");

            var result = await _service.CreateGroup(newGroupRequest);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateGroup_UpdatesGroup_WhenFound()
        {
            var oldGroup = new Group { Name = "AMT" };
            _db.Groups.Add(oldGroup);
            await _db.SaveChangesAsync();
            var updateGroupRequest = new GroupDto("Aviation Maintenance Training");

            var result = await _service.UpdateGroup(oldGroup.Id, updateGroupRequest);

            Assert.NotNull(result);
            Assert.Equal(oldGroup.Name, result.Name);
            var updatedGroup = await _db.Groups.SingleAsync(g => g.Id == result.Id);
            Assert.NotNull(updatedGroup);
            Assert.Equal("Aviation Maintenance Training", updatedGroup.Name);
        }

        // TODO: UpdateGroup - returns null when the group doesn't exist

        // TODO: DeleteGroup - returns NotFound when the group doesn't exist


        [Fact]
        public async Task DeleteGroup_ReturnsInUse_WhenResourceBelongsToGroup()
        {
            var group = new Group { Name = "AMT" };
            var resourceType = new ResourceType { Name = "Exam-Room" };
            await _db.Groups.AddAsync(group);
            await _db.ResourceTypes.AddAsync(resourceType);
            _db.Resources.Add(new Resource { Name = "PTS-214",TotalCapacity = 26, GroupId = group.Id , ReservationMode = ReservationMode.exclusive, ResourceTypeId = resourceType.Id});
            await _db.SaveChangesAsync();

            var result = await _service.DeleteGroup(group.Id);

            Assert.Equal(DbOperationResult.InUse, result);
        }

        // TODO: DeleteGroup - deletes the group when it isn't being used
    }
}
