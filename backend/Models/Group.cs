using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ResourceManagerAPI.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Group : BaseAuditableEntity
    {
        [MaxLength(255)]
        public required string Name { get; set; }

        public ICollection<User> Users { get; set; } = [];
        public ICollection<Resource> Resources { get; set; } = [];
    }
}
