namespace ResourceManagerAPI.Routes
{
    public static class RouteHandler
    {
        public static void MapRoutes(this WebApplication app)
        {
            app.MapUserRoutes();

        }
    }
}
