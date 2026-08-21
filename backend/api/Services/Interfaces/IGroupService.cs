using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Models;

namespace ResourceManagerAPI.Services.Interfaces
{
    public interface IGroupService
    {
        Task<List<Group>> GetAllGroups();
        Task<Group?> GetGroupById(Guid id);
        Task<Group?> CreateGroup(GroupDto newGroupRequest);
        Task<Group?> UpdateGroup(Guid id, GroupDto UpdatedGroup);
        Task<DbOperationResult> DeleteGroup(Guid id);

    }
}
