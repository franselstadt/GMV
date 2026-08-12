using System;
using System.Collections.Generic;
using gmvTM.Domain.Items;
using gmvTM.Domain.Items.View;

namespace gmvTM.Application.Classes.Simulation
{
    public sealed class ActiveSimulation
    {
        private readonly int id;
        private readonly string routeCode;
        private readonly int vehicleID;
        private readonly string vehicleNumber;
        private int tripID;
        private string status;
        private string startStopCode;
        private readonly double averageMph;
        private readonly int averageDwellSeconds;
        private DateTime startedAtUtc;
        private int startStopIndex;
        private readonly double speedMetersPerSecond;
        private readonly RoutePathViewItem path;
        private readonly IReadOnlyList<PathStopViewItem> stops;
        private List<StopTripItem> stopTrips;
        private string? scheduleAlert;

        public ActiveSimulation(int id, string routeCode, int vehicleID, string vehicleNumber, int tripID, string status, string startStopCode, double averageMph, int averageDwellSeconds, DateTime startedAtUtc, int startStopIndex, double speedMetersPerSecond, RoutePathViewItem path, IReadOnlyList<PathStopViewItem> stops, List<StopTripItem>? stopTrips = null)
        {
            this.id = id;
            this.routeCode = routeCode ?? throw new ArgumentNullException(nameof(routeCode));
            this.vehicleID = vehicleID;
            this.vehicleNumber = vehicleNumber ?? throw new ArgumentNullException(nameof(vehicleNumber));
            this.tripID = tripID;
            this.status = status ?? throw new ArgumentNullException(nameof(status));
            this.startStopCode = startStopCode ?? throw new ArgumentNullException(nameof(startStopCode));
            this.averageMph = averageMph;
            this.averageDwellSeconds = averageDwellSeconds;
            this.startedAtUtc = startedAtUtc;
            this.startStopIndex = startStopIndex;
            this.speedMetersPerSecond = speedMetersPerSecond;
            this.path = path ?? throw new ArgumentNullException(nameof(path));
            this.stops = stops ?? throw new ArgumentNullException(nameof(stops));
            this.stopTrips = stopTrips ?? new List<StopTripItem>();
        }

        public int ID
        {
            get { return this.id; }
        }

        public string RouteCode
        {
            get { return this.routeCode; }
        }

        public int VehicleID
        {
            get { return this.vehicleID; }
        }

        public string VehicleNumber
        {
            get { return this.vehicleNumber; }
        }

        public int TripID
        {
            get { return this.tripID; }
            set { this.tripID = value; }
        }

        public string Status
        {
            get { return this.status; }
            set { this.status = value; }
        }

        public string StartStopCode
        {
            get { return this.startStopCode; }
            set { this.startStopCode = value; }
        }

        public double AverageMph
        {
            get { return this.averageMph; }
        }

        public int AverageDwellSeconds
        {
            get { return this.averageDwellSeconds; }
        }

        public DateTime StartedAtUtc
        {
            get { return this.startedAtUtc; }
            set { this.startedAtUtc = value; }
        }

        public int StartStopIndex
        {
            get { return this.startStopIndex; }
            set { this.startStopIndex = value; }
        }

        public double SpeedMetersPerSecond
        {
            get { return this.speedMetersPerSecond; }
        }

        public RoutePathViewItem Path
        {
            get { return this.path; }
        }

        public IReadOnlyList<PathStopViewItem> Stops
        {
            get { return this.stops; }
        }

        public List<StopTripItem> StopTrips
        {
            get { return this.stopTrips; }
            set { this.stopTrips = value; }
        }

        public string? ScheduleAlert
        {
            get { return this.scheduleAlert; }
            set { this.scheduleAlert = value; }
        }
    }
}
