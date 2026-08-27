using Microsoft.EntityFrameworkCore;
using ResourceManagerAPI.Data;
using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Extensions;
using ResourceManagerAPI.Models;
using ResourceManagerAPI.Services.Interfaces;

namespace ResourceManagerAPI.Services.Implementations
{
    public class ReservationService(AppDbContext db, ICurrentUserService currentUser, IEnumerable<IReservationValidator> validators) : IReservationService
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

            if (!isOwnGroup && !await currentUser.IsCreatorOfAsync(reservation)) return null;
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

        private async Task<bool> IsValidReservationRequest(ReservationRequest request, Guid? id = null){
            foreach (var validator in validators){
                if (!await validator.IsValid(request, id)) return false;
            }
            return true;
        }
    }
}
