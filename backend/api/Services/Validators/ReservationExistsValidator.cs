using Microsoft.EntityFrameworkCore;
using ResourceManagerAPI.Data;
using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Services.Interfaces;

namespace ResourceManagerAPI.Services.Validators
{
    public class ReservationExistsValidator(AppDbContext db) : IReservationValidator
    {
        public async Task<bool> IsValid(ReservationRequest request, Guid? reservationId = null)
        {
            if (reservationId is not Guid id) return true;
            return await db.Reservations.AnyAsync(r => r.Id == id);
        }

    }
}
