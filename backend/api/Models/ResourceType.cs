using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ResourceManagerAPI.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class ResourceType : BaseAuditableEntity
    {
        [MaxLength (100)]
        public required string Name { get; set; }

        public ICollection<Resource> Resources { get; set; } = [];
    }
}
