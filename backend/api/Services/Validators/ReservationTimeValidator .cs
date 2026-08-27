using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Services.Interfaces;

namespace ResourceManagerAPI.Services.Validators
{
    public class ReservationTimeValidator : IReservationValidator
    {
        public Task<bool> IsValid(ReservationRequest request, Guid? reservationId = null)
        {
            var valid = request.Start >= DateTimeOffset.UtcNow && request.Start < request.End;

            return Task.FromResult(valid);
        }
    }
}
