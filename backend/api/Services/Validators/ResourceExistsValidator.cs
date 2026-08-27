using Microsoft.EntityFrameworkCore;
using ResourceManagerAPI.Data;
using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Services.Interfaces;

namespace ResourceManagerAPI.Services.Validators
{
    public class ResourceExistsValidator(AppDbContext db) : IReservationValidator
    {
        public async Task<bool> IsValid( ReservationRequest request, Guid? reservationId = null)
        {
            return await db.Resources.AnyAsync(r => r.Id == request.ResourceId);
        }
    }
}
