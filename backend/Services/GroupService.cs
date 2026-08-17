using Microsoft.EntityFrameworkCore;
using ResourceManagerAPI.Data;
using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Models;

namespace ResourceManagerAPI.Services
{
    public class GroupService(AppDbContext db)
    {
        public async Task<List<Group>> GetAllGroups()
        {
            var groups = await db.Groups.ToListAsync();

            return groups;
        }
        public async Task<Group?> GetGroupById(Guid id)
        {
            var group = await db.Groups.FindAsync(id);

            return group is null ? null : group;
        }

        public async Task<Group> CreateGroup(GroupDTO newGroupRequest)
        {
            var newGroup = new Group { Name = newGroupRequest.Name };

            db.Groups.Add(newGroup);
            await db.SaveChangesAsync();

            return newGroup;
        }
        public async Task<Group?> UpdateGroup(Guid id, GroupDTO UpdatedGroup)
        {
            var group = await db.Groups.FindAsync(id);
            if (group is null) return null;

            group.Name = UpdatedGroup.Name;
            await db.SaveChangesAsync();

            return group;
        }
        public async Task<bool> DeleteGroup(Guid id)
        {
            var group = await db.Groups.FindAsync(id);
            if (group is null) return false;

            db.Groups.Remove(group);
            db.SaveChanges();
            return true;
        }
    }
}
