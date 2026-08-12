namespace gmvTM.Domain
{
    public readonly struct Messages
    {
        public string StopNotFound => "Stop '{0}' was not found on route '{1}'.";
        public string RouteNotFound => "Route '{0}' was not found.";
        public string RouteCodeRequired => "Route code is required.";
        public string PageMustBePositive => "Page must be greater than or equal to 1.";
        public string PageSizeMustBePositive => "Page size must be greater than or equal to 1.";
        public string CountMustBePositive => "Count must be greater than or equal to 1.";
        public string StopCodeRequired => "Stop code is required.";
        public string AppTitle => "GMV Time and Motion";
        public string NextArrival => "Next arrival";
        public string SelectStop => "Stop";
        public string NoUpcomingArrivals => "No upcoming arrivals.";
        public string MphMustBePositive => "Average miles per hour must be greater than 0.";
        public string MphOutOfRange => "Average miles per hour must be between {0} and {1}.";
        public string AverageDwellMustBeAtLeast => "Average dwell seconds must be at least {0}.";
        public string AverageDwellOutOfRange => "Average dwell seconds must be between {0} and {1}.";
        public string NoScheduleForRoute => "No schedule is available for route '{0}'.";
        public string DefaultRunLabelFormat => "{0} {1}";
        public string NoVehicleAvailable => "No vehicle is available in the fleet.";
        public string VehicleNotFound => "Vehicle '{0}' was not found.";
        public string StopNotOnSchedule => "Stop '{0}' is not on the schedule for route '{1}' (or is the last stop).";
        public string SimulationNotFound => "Simulation '{0}' was not found.";
        public string SimulationStatusRunning => "Running";
        public string SimulationStatusStopped => "Stopped";
        public string SimulationStatusCompleted => "Completed";
        public string BehindScheduleAlert => "Behind schedule at {0}: actual {1}s from start, planned {2}s.";
        public string SimulationRequiredForArrivals => "Start a simulation before calculating next arrivals.";
        public string OrmTypeNotSupported => "ORM type '{0}' is not supported.";
        public string ItemCannotBeCreatedThroughCollection => "Item type '{0}' cannot be created through '{1}' collection.";
        public string ItemCannotBeUpdatedThroughCollection => "Item type '{0}' cannot be updated through '{1}' collection.";
        public string ForeignKeyPrincipalNotPersistable => "'{0}.{1}' references '{2}' which is not a persistable item.";
        public string FactoryPhantomRequired => "A phantom object is required to create an item.";
        public string FactoryPropertyNotFound => "Phantom property '{0}' does not match any writable property on '{1}'.";
        public string FactoryPropertyTypeMismatch => "Phantom property '{0}' of type '{1}' cannot be assigned to '{2}.{3}' of type '{4}'.";
        public string LogFactoryDynamicPropertyAdded => "Phantom property {PropertyName} does not exist on {DtoType}; carried as a dynamic property.";
        public string TripNeedsTwoStops => "A trip needs at least two stops.";
        public string RoutePathNeedsTwoPoints => "A route path needs at least two points.";
        public string CumulativeDistancesMustMatchPoints => "Cumulative distances must match point count.";
        public string ScheduleNeedsTwoStops => "Schedule on route '{0}' needs at least two stops.";
        public string PathIndicesOutOfOrder => "Path indices must be in trip order (toIndex >= fromIndex).";
        public string InvalidEncodedPolyline => "Invalid encoded polyline.";
        public string SeedFileDeserializeFailed => "Failed to deserialize seed file '{0}'.";
        public string SeedUnknownStopCode => "Scheduled stop references unknown stop code '{0}'.";
        public string NoBaselineArrivalSeconds => "No baseline arrival seconds for sequence {0}.";
        public string RouteNeedsTwoStopsForArrivalSeconds => "Route needs at least two stops to build arrival seconds.";
        public string SeedFileNotFound => "Could not find '{0}'. Expected under '{1}/' next to the assembly.";
        public string LatitudeOutOfRange => "Latitude must be between -90 and 90.";
        public string LongitudeOutOfRange => "Longitude must be between -180 and 180.";
        public string LogDatabaseAlreadySeeded => "Database already seeded";
        public string LogSeededRoute => "Seeded route {ShortName} with vehicle {FleetCode}, {StopCount} stops, and {ScheduleCount} scheduled stops (arrival seconds at {Mph} mph + {Dwell}s dwell).";
    }
}
