using Microsoft.EntityFrameworkCore;
using ResourceManagerAPI.Data;
using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Services.Interfaces;

namespace ResourceManagerAPI.Services.Validators
{
    public class ReservationCapacityValidator(AppDbContext db) : IReservationValidator
    {
        public async Task<bool> IsValid(ReservationRequest request, Guid? reservationId = null)
        {
            var resourceCapacity = await db.Resources.Where(r => r.Id == request.ResourceId).Select(r => r.TotalCapacity).SingleAsync();

            var alreadyBooked = await db.Reservations
                .Where(r =>
                    r.ResourceId == request.ResourceId &&
                    r.Start < request.End &&
                    r.End > request.Start &&
                    r.Status == ReservationStatus.Confirmed &&
                    (!reservationId.HasValue || r.Id != reservationId.Value))
                .SumAsync(r => r.BookedCapacity);

            var hasCapacity = resourceCapacity - alreadyBooked >= request.BookedCapacity;

            return hasCapacity;
        }
    }
}
