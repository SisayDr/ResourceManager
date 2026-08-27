using ResourceManagerAPI.DTOs;

namespace ResourceManagerAPI.Services.Interfaces
{
    public interface IReservationValidator
    {
        Task<bool> IsValid(ReservationRequest request, Guid? reservationId = null);
    }
}
