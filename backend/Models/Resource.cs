using System.ComponentModel.DataAnnotations;

namespace ResourceManagerAPI.Models
{
    public class Resource : BaseAuditableEntity
    {
        [MaxLength(255)]
        public required string Name { get; set; }
        [Range(1, int.MaxValue)]
        public int TotalCapacity { get; set; }
        public required ReservationMode ReservationMode { get; set; }

        //TODO: Add Tags Property/Model to give more information about a Reource like "3rd Floor", "WiFi", "Projector"...

        //TODO: ADD OpenHours Property to support proper reservations.

        public Guid ResourceTypeId { get; set; }
        public ResourceType ResourceType { get; set; } = null!;

        public Guid GroupId { get; set; }
        public Group Group { get; set; } = null!;

        public ICollection<Reservation> Reservations { get; set; } = [];
    }
}
