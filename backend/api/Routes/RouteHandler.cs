namespace ResourceManagerAPI.Routes
{
    public static class RouteHandler
    {
        public static void MapRoutes(this WebApplication app){
            app.MapUserRoutes();
            app.MapGroupRoutes();
            app.MapResourceTypeRoutes();
            app.MapResourceRoutes();
            app.MapReservationRoutes();

        }
    }
}
