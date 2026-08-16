using System.ComponentModel.DataAnnotations;

namespace ResourceManagerAPI.Models
{
    public class ResourceType : BaseAuditableEntity
    {
        [MaxLength (100)]
        public required string Name { get; set; }

        public ICollection<Resource> Resources { get; set; } = [];
    }
}
