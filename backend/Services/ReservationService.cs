using Microsoft.EntityFrameworkCore;
using ResourceManagerAPI.Data;
using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Extensions;
using ResourceManagerAPI.Models;

namespace ResourceManagerAPI.Services
{
    public class ReservationService(AppDbContext db, CurrentUserService currentUser)
    {
        public async Task<List<ReservationResponse>> GetAllReservations() {
            var reservations = await db.Reservations.Include(r => r.Resource).Include(r => r.User).ToListAsync();
            var result = new List<ReservationResponse>();

            foreach (var reservation in reservations) {
                if(await currentUser.CanAccessGroupAsync(reservation.Resource.GroupId) || await currentUser.IsCreatorOfAsync(reservation))
                    result.Add(reservation.ToReservationResponse());
            } 
            return result;
        }
        public async Task<List<ReservationResponse>> GetPendingReservations()
        {
            var pendingReservations = await db.Reservations.Where(r => r.Status == ReservationStatus.Pending).Include(r => r.Resource).Include(r => r.User).ToListAsync();
            var result = new List<ReservationResponse>();

            foreach (var reservation in pendingReservations)
            {
                if (await currentUser.CanAccessGroupAsync(reservation.Resource.GroupId) || await currentUser.IsCreatorOfAsync(reservation))
                    result.Add(reservation.ToReservationResponse());
            }
            return result;
        }
        public async Task<ReservationResponse?> GetReservationById(Guid id)
        {
            var reservation = await db.Reservations.Include(r => r.Resource).Include(r => r.User).FirstOrDefaultAsync(r => r.Id == id);

            if (reservation is null) return null;

            return (await currentUser.CanAccessGroupAsync(reservation.Resource.GroupId) || await currentUser.IsCreatorOfAsync(reservation)) ? reservation.ToReservationResponse() : null;

        }
        public async Task<ReservationResponse?> CreateReservation(ReservationRequest request) {
            if(!await IsValidReservationRequest(request)) return null;

            var groupId = (await db.Resources.FindAsync(request.ResourceId)).GroupId;

            Reservation newReservation = request.ToReservation();

            newReservation.Status = (await currentUser.CanAccessGroupAsync(groupId)) ? (ReservationStatus.Confirmed) : (ReservationStatus.Pending);
        
            await db.Reservations.AddAsync(newReservation);
            await db.SaveChangesAsync();

            return newReservation.ToReservationResponse();
        }

        public async Task<ReservationResponse?> UpdateReservation(Guid id, ReservationRequest request)
        {
            if (!await IsValidReservationRequest(request, id)) return null;

            var reservation = await db.Reservations.Include(r => r.Resource).Include(r => r.User).FirstOrDefaultAsync(r => r.Id == id);
            var isOwnGroup = await currentUser.CanAccessGroupAsync(reservation.Resource.GroupId);

            if (!isOwnGroup || !await currentUser.IsCreatorOfAsync(reservation)) return null;
            reservation.Start = request.Start;
            reservation.End = request.End;
            reservation.BookedCapacity = request.BookedCapacity;

            if (isOwnGroup) reservation.Status = (ReservationStatus) request.Status!;

            reservation.ResourceId = request.ResourceId;
            await db.SaveChangesAsync();
            return reservation.ToReservationResponse();
        }

        public async Task<DbOperationResult> DeleteReservation(Guid id)
        {
            var reservation = await db.Reservations.FindAsync(id);
            if(reservation is null) return DbOperationResult.NotFound;

            if(!await currentUser.IsCreatorOfAsync(reservation)) return DbOperationResult.UnAuthorized;

            db.Reservations.Remove(reservation);
            await db.SaveChangesAsync();

            return DbOperationResult.Deleted;
        }

        private async Task<bool> IsValidReservationRequest (ReservationRequest request, Guid? id = null)
        {
            if (request.Start < DateTimeOffset.UtcNow) return false;
            if (request.Start >= request.End) return false;

            if(!await db.Resources.AnyAsync(r => r.Id == request.ResourceId)) return false;

            if (id is Guid reservationId && !await db.Reservations.AnyAsync(r => r.Id == reservationId)) return false;

            var resourceCapacity = await db.Resources.Where(r => r.Id == request.ResourceId).Select(r => r.TotalCapacity).SingleAsync();
            var alreadyBooked = await db.Reservations
                .Where(r =>
                       r.ResourceId == request.ResourceId &&
                       r.Start < request.End && r.End > request.Start &&
                       r.Status == ReservationStatus.Confirmed &&
                       (!id.HasValue || r.Id != id.Value))
                .SumAsync(r => r.BookedCapacity);

            var hasCapacity = resourceCapacity - alreadyBooked >= request.BookedCapacity;

            if (!hasCapacity) return false;

            return true;
        }
    }
}
