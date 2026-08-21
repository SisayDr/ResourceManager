using ResourceManagerAPI.DTOs;

namespace ResourceManagerAPI.Services.Interfaces
{
    public interface IReservationService
    {
        Task<List<ReservationResponse>> GetAllReservations();
        Task<List<ReservationResponse>> GetPendingReservations();
        Task<ReservationResponse?> GetReservationById(Guid id);
        Task<ReservationResponse?> CreateReservation(ReservationRequest request);
        Task<ReservationResponse?> UpdateReservation(Guid id, ReservationRequest request);
        Task<DbOperationResult> DeleteReservation(Guid id);
    }
}
