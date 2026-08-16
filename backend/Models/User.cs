using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ResourceManagerAPI.Models
{
    public class User : IdentityUser
    {
        [MaxLength(100)]
        public required string FullName { get; set; }
        public Guid? GroupId { get; set; }
        public Group? Group { get; set; }
        public ICollection<Reservation> Reservations { get; set; } = [];

    }
}
