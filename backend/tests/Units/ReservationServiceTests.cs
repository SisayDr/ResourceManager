using System;
using System.Collections.Generic;
using System.Text;

namespace ResourceManagerAPI.Tests.Units
{
    internal class ReservationServiceTests
    {
        // TODO: GetAllReservations - returns only reservations the current user is allowed to access

        // TODO: GetAllReservations - includes reservations created by the current user even without group access


        // TODO: GetPendingReservations - returns only pending reservations the current user is allowed to access


        // TODO: GetReservationById - returns the reservation when the user has access

        // TODO: GetReservationById - returns null when the reservation doesn't exist or the user isn't authorized


        // TODO: CreateReservation - rejects an invalid reservation request

        // TODO: CreateReservation - creates a Confirmed reservation when the user has group access

        // TODO: CreateReservation - creates a Pending reservation when the user doesn't have group access


        // TODO: UpdateReservation - rejects an invalid reservation request

        // TODO: UpdateReservation - allows the creator or group member to update the reservation

        // TODO: UpdateReservation - prevents unauthorized users from updating the reservation


        // TODO: DeleteReservation - returns NotFound when the reservation doesn't exist

        // TODO: DeleteReservation - prevents users who aren't the creator from deleting

        // TODO: DeleteReservation - deletes the reservation when the creator requests it


        // TODO: Reservation validation - rejects overlapping reservations when there isn't enough capacity

        // TODO: Reservation validation - allows a reservation when there is enough capacity
    }
}
