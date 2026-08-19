using ResourceManagerAPI.DTOs;
using ResourceManagerAPI.Services;

namespace ResourceManagerAPI.Routes
{
    public static class ReservationRoutes
    {
        public static void MapReservationRoutes(this WebApplication app)
        {
            var router = app.MapGroup("/api/reservations").RequireAuthorization();

            router.MapGet("/", GetAllReservations);
            router.MapGet("/pending", GetPendingReservations);
            router.MapGet("/{id}", GetReservation);
            router.MapPost("/", CreateReservation);
            router.MapPut("/{id}", UpdateReservation);
            router.MapDelete("/{id}", DeleteReservation);
        }

        public static async Task<IResult> GetAllReservations(ReservationService service)
        {
            var result = await service.GetAllReservations();
            return Results.Ok(result);
        }
        public static async Task<IResult> GetReservation(Guid id, ReservationService service)
        {
            var result = await service.GetReservationById(id);
            return result is null ? Results.NotFound() : Results.Ok(result);
        }
        public static async Task<IResult> GetPendingReservations(ReservationService service)
        {
            var result = await service.GetPendingReservations();
            return result is null ? Results.NotFound() : Results.Ok(result);
        }
        public static async Task<IResult> CreateReservation(ReservationRequest request, ReservationService service)
        {
            var result = await service.CreateReservation(request);
            return result is null ? Results.BadRequest() : Results.Ok(result);
        }
        public static async Task<IResult> UpdateReservation(Guid id, ReservationRequest request, ReservationService service)
        {
            var result = await service.UpdateReservation(id, request);
            return result is null ? Results.BadRequest() : Results.Ok(result);
        }
        public static async Task<IResult> DeleteReservation(Guid id, ReservationService service)
        {
            var result = await service.DeleteReservation(id);
            return result switch
            {
                DbOperationResult.NotFound => Results.NotFound(),
                DbOperationResult.UnAuthorized => Results.Unauthorized(),
                DbOperationResult.Deleted => Results.NoContent(),
                _ => Results.Problem()
            };
        }
    }
}
