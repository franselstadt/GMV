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
    }
}
