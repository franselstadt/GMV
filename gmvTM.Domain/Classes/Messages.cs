namespace gmvTM.Domain
{
    public readonly struct Messages
    {
        public const string StopNotFound = "Stop '{0}' was not found on route '{1}'.";
        public const string RouteNotFound = "Route '{0}' was not found.";
        public const string RouteCodeRequired = "Route code is required.";
        public const string PageMustBePositive = "Page must be greater than or equal to 1.";
        public const string PageSizeMustBePositive = "Page size must be greater than or equal to 1.";
        public const string CountMustBePositive = "Count must be greater than or equal to 1.";
        public const string StopCodeRequired = "Stop code is required.";
        public const string AppTitle = "GMV Time and Motion";
        public const string NextArrival = "Next arrival";
        public const string SelectStop = "Stop";
        public const string NoUpcomingArrivals = "No upcoming arrivals.";
        public const string MphMustBePositive = "Average miles per hour must be greater than 0.";
        public const string MphOutOfRange = "Average miles per hour must be between {0} and {1}.";
        public const string AverageDwellMustBeAtLeast = "Average dwell seconds must be at least {0}.";
        public const string AverageDwellOutOfRange = "Average dwell seconds must be between {0} and {1}.";
        public const string NoScheduleForRoute = "No schedule is available for route '{0}'.";
        public const string DefaultRunLabelFormat = "{0} {1}";
        public const string NoVehicleAvailable = "No vehicle is available in the fleet.";
        public const string VehicleNotFound = "Vehicle '{0}' was not found.";
        public const string StopNotOnSchedule = "Stop '{0}' is not on the schedule for route '{1}' (or is the last stop).";
        public const string SimulationNotFound = "Simulation '{0}' was not found.";
        public const string SimulationStatusRunning = "Running";
        public const string SimulationStatusStopped = "Stopped";
        public const string SimulationStatusCompleted = "Completed";
        public const string BehindScheduleAlert = "Behind schedule at {0}: actual {1}s from start, planned {2}s.";
        public const string SimulationRequiredForArrivals = "Start a simulation before calculating next arrivals.";
        public const string ItemCannotBeCreatedThroughCollection = "Item type '{0}' cannot be created through '{1}' collection.";
        public const string ItemCannotBeUpdatedThroughCollection = "Item type '{0}' cannot be updated through '{1}' collection.";
        public const string ForeignKeyPrincipalNotPersistable = "'{0}.{1}' references '{2}' which is not a persistable item.";
        public const string TripNeedsTwoStops = "A trip needs at least two stops.";
        public const string RoutePathNeedsTwoPoints = "A route path needs at least two points.";
        public const string CumulativeDistancesMustMatchPoints = "Cumulative distances must match point count.";
        public const string ScheduleNeedsTwoStops = "Schedule on route '{0}' needs at least two stops.";
        public const string PathIndicesOutOfOrder = "Path indices must be in trip order (toIndex >= fromIndex).";
        public const string InvalidEncodedPolyline = "Invalid encoded polyline.";
        public const string SeedFileDeserializeFailed = "Failed to deserialize seed file '{0}'.";
        public const string SeedUnknownStopCode = "Scheduled stop references unknown stop code '{0}'.";
        public const string NoBaselineArrivalSeconds = "No baseline arrival seconds for sequence {0}.";
        public const string RouteNeedsTwoStopsForArrivalSeconds = "Route needs at least two stops to build arrival seconds.";
        public const string SeedFileNotFound = "Could not find '{0}'. Expected under '{1}/' next to the assembly.";
        public const string LatitudeOutOfRange = "Latitude must be between -90 and 90.";
        public const string LongitudeOutOfRange = "Longitude must be between -180 and 180.";
        public const string LogDatabaseAlreadySeeded = "Database already seeded";
        public const string LogSeededRoute = "Seeded route {ShortName} with vehicle {FleetCode}, {StopCount} stops, and {ScheduleCount} scheduled stops (arrival seconds at {Mph} mph + {Dwell}s dwell).";
    }
}
