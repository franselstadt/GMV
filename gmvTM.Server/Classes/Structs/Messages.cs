namespace gmvTM.Server
{
    public readonly struct Messages
    {
        public string ErrorTitleValidation => "Validation failed";
        public string ErrorTitleNotFound => "Resource not found";
        public string ErrorTitleBadRequest => "Bad request";
        public string ErrorTitleUnexpected => "An unexpected error occurred";
        public string UnexpectedError => "An unexpected error occurred.";
        public string ResponseAlreadyStarted => "The response has already started; cannot write error envelope.";
        public string SwaggerDescription => "GMV Time and Motion â€” Route F next-arrival and simple vehicle simulation.";
        public string SwaggerUiDisplayName => "gmvTM v1";
        public string LogStartingApi => "Starting gmvTM API in {Environment} mode.";
        public string LogApiConfigured => "gmvTM API configured; listening for requests.";
        public string LogUnhandledException => "Unhandled exception for {TraceId}";
        public string LogSimulationTickFailed => "Simulation tick failed.";
        public string LogListingRoutes => "Listing all routes.";
        public string LogReturningRoutes => "Returning {RouteCount} routes.";
        public string LogGettingRoute => "Getting route {RouteCode}.";
        public string LogGettingRouteShape => "Getting shape for route {RouteCode}.";
        public string LogRouteShapePoints => "Route {RouteCode} shape contains {PointCount} points.";
        public string LogListingVehicles => "Listing vehicles for route {RouteCode}.";
        public string LogListingFleet => "Listing all vehicles in the fleet.";
        public string LogReturningFleet => "Returning {VehicleCount} vehicles.";
        public string LogListingStops => "Listing stops for route {RouteCode} (page {Page}, page size {PageSize}).";
        public string LogReturningStops => "Returning {StopCount} of {TotalCount} stops for route {RouteCode}.";
        public string LogGettingStop => "Getting stop {StopCode} on route {RouteCode}.";
        public string LogCalculatingNextArrival => "Calculating next arrival at stop {StopCode} on route {RouteCode}.";
        public string LogNoUpcomingArrival => "No upcoming arrival found at stop {StopCode} on route {RouteCode}.";
        public string LogNextArrival => "Next arrival at stop {StopCode} on route {RouteCode}: run {RunLabel}, planned {PlannedTime}, status {Status}.";
        public string LogStartingSimulation => "Starting simulation on route {RouteCode} from stop {StopCode} ({AverageMph} mph, {AverageDwellSeconds}s dwell).";
        public string LogSimulationStarted => "Simulation {SimulationId} started on route {RouteCode} with vehicle {VehicleNumber} (trip {TripId}).";
        public string LogListingSimulations => "Listing active simulations.";
        public string LogReturningSimulations => "Returning {SimulationCount} active simulations.";
        public string LogStoppingSimulation => "Stopping simulation {SimulationId}.";
        public string LogSimulationStopped => "Simulation {SimulationId} stopped.";
        public string LogReseedingDatabase => "Clearing and reseeding the database.";
        public string LogDatabaseReseeded => "Database cleared and reseeded.";
        public string ErrorTitleUnauthorized => "Authentication required";
        public string SwaggerBasicAuthDescription => "Simple HTTP Basic authentication for this exam project.";
    }
}
