using Microsoft.AspNetCore.Identity;
using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Models;

namespace ResourceManagerAPI.Extensions
{
    public static class ReservationExtensions
    {
        public static ReservationResponse ToReservationResponse(this Reservation reservation)
        {
            return new ReservationResponse(reservation.Id, reservation.Start, reservation.End, reservation.BookedCapacity, reservation.Status, reservation.Resource.Name, reservation.User.FullName);
        }
        public static Reservation ToReservation(this ReservationRequest request)
        {
            return new Reservation
            {
                Start = request.Start,
                End = request.End,
                BookedCapacity = request.BookedCapacity,
                Status = (ReservationStatus.Pending),
                ResourceId = request.ResourceId
            };
        }
    }
}
