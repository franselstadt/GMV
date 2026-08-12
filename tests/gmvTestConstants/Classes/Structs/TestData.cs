using System;

namespace gmvTestConstants
{
    public readonly struct TestData
    {
        public string RouteCode => "F";
        public string RouteLongName => "DASH F";
        public string EncodedPolyline => "_p~iF~ps|U";
        public string OriginStopCode => "F00";
        public string OriginStopName => "A";
        public string DepotStopCode => "F01";
        public string DepotStopName => "Depot";
        public string VehicleNumber => "DASH-F-1";
        public string SimulationStartStopCode => "6041";
        public string NextArrivalStopCode => "6138";
        public double AverageMph => 25;
        public double UnitTestAverageMph => 18;
        public int AverageDwellSeconds => 12;
        public DateTime FixedTestClockUtc => new DateTime(2026, 8, 10, 6, 0, 0, DateTimeKind.Utc);
    }
}
