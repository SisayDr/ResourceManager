using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceManagerAPI.Models
{
    public class Reservation : BaseAuditableEntity
    {
        public required DateTimeOffset Start { get; set; }
        public required DateTimeOffset End { get; set; }
        [Range(1, int.MaxValue)]
        public required int BookedCapacity { get; set; }
        public required ReservationStatus Status { get; set; }

        public required Guid ResourceId { get; set; }
        public Resource Resource { get; set; } = null!;

        [ForeignKey(nameof(CreatedBy))]
        public User User { get; set; } = null!;
    }
}
