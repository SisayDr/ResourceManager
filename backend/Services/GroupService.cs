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

        public async Task<Group?> CreateGroup(GroupDto newGroupRequest)
        {
            var alreadyExist = await db.Groups.AnyAsync(g => g.Name == newGroupRequest.Name);
            if (alreadyExist) return null;

            var newGroup = new Group { Name = newGroupRequest.Name };
            await db.Groups.AddAsync(newGroup);
            await db.SaveChangesAsync();

            return newGroup;
        }
        public async Task<Group?> UpdateGroup(Guid id, GroupDto UpdatedGroup)
        {
            var group = await db.Groups.FindAsync(id);
            if (group is null) return null;

            group.Name = UpdatedGroup.Name;
            await db.SaveChangesAsync();

            return group;
        }
        public async Task<DbOperationResult> DeleteGroup(Guid id)
        {
            var group = await db.Groups.FindAsync(id);
            if (group is null) return DbOperationResult.NotFound;

            var isInUse = await db.Resources.AnyAsync(r => r.GroupId == id) || await db.Users.AnyAsync(u => u.GroupId == id);
            if(isInUse) return DbOperationResult.InUse;

            db.Groups.Remove(group);
            db.SaveChanges();
            return DbOperationResult.Deleted;
        }

    }
}
