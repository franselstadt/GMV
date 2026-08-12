using System;

namespace gmvTM.Domain
{
    public readonly struct AppConstants
    {
        public string CorsPolicyName => "Default";
        public string VehiclePositionHubPath => "/hubs/vehicle-position";
        public string VehiclePositionEvent => "positionUpdate";
        public string VehiclePositionGroupPrefix => "vehicle-";

        public string VehicleGroupName(string fleetCode) => $"{VehiclePositionGroupPrefix}{fleetCode.Trim().ToUpperInvariant()}";
        public string DefaultRouteCode => "F";
        public double DefaultAverageMph => 25;
        public double MinAverageMph => 10;
        public double MaxAverageMph => 35;
        public int DefaultAverageDwellSeconds => 12;
        public int MinAverageDwellSeconds => 5;
        public int MaxAverageDwellSeconds => 120;
        public int AnnounceLeadSeconds => 30;
        public int DoorClosingSeconds => 1;
        public int SimulationTickMilliseconds => 1000;
        public int ScheduleGraceSeconds => 60;
        public double MetersPerMile => 1609.344;
        public double SecondsPerHour => 3600;

        public double MetersPerSecondFromMph(double mph) => mph * MetersPerMile / SecondsPerHour;

        public int ArrivalSecondsFromPrevious(double metersFromPrevious, double mph, int dwellSeconds)
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
