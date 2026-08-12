using System;

namespace gmvTM.Domain
{
    public static class AppConstants
    {
        public const string CorsAllowedOriginsSection = "Cors:AllowedOrigins";
        public const string CorsPolicyName = "Default";
        public const string VehiclePositionHubPath = "/hubs/vehicle-position";
        public const string VehiclePositionEvent = "positionUpdate";
        public const string DefaultRouteCode = "F";
        public const double DefaultAverageMph = 25;
        public const double MinAverageMph = 10;
        public const double MaxAverageMph = 35;
        public const int DefaultAverageDwellSeconds = 12;
        public const int MinAverageDwellSeconds = 5;
        public const int MaxAverageDwellSeconds = 120;
        public const int AnnounceLeadSeconds = 30;
        public const int DoorClosingSeconds = 4;
        public const int SimulationTickMilliseconds = 1000;
        public const int ScheduleGraceSeconds = 60;
        public const double MetersPerMile = 1609.344;
        public const double SecondsPerHour = 3600;

        public static double MetersPerSecondFromMph(double mph)
        {
            return mph * MetersPerMile / SecondsPerHour;
        }

        public static int ArrivalSecondsFromPrevious(double metersFromPrevious, double mph, int dwellSeconds)
        {
            if (metersFromPrevious < 0)
                metersFromPrevious = 0;

            double metersPerSecond = MetersPerSecondFromMph(mph);

            int travelSeconds = metersPerSecond <= 0
                ? 0
                : (int)Math.Round(metersFromPrevious / metersPerSecond);

            return dwellSeconds + travelSeconds;
        }
    }
}
