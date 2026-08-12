using gmvTM.Domain.Items.Base;
using System;

namespace gmvTM.Domain.Items.View
{
    public sealed class PathStopViewItem : ViewItem
    {
        private readonly int stopID;
        private readonly string stopCode;
        private readonly string name;
        private readonly double distanceAlongPathMeters;
        private readonly int plannedArrivalSeconds;

        public PathStopViewItem(int stopID, string stopCode, string name, double distanceAlongPathMeters, int plannedArrivalSeconds)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(stopCode);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            this.stopID = stopID;
            this.stopCode = stopCode;
            this.name = name;
            this.distanceAlongPathMeters = distanceAlongPathMeters;
            this.plannedArrivalSeconds = plannedArrivalSeconds;
        }

        public override string ViewName
        {
            get { return "PathStop"; }
        }

        public int StopID
        {
            get { return this.stopID; }
        }

        public string StopCode
        {
            get { return this.stopCode; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public double DistanceAlongPathMeters
        {
            get { return this.distanceAlongPathMeters; }
        }

        public int PlannedArrivalSeconds
        {
            get { return this.plannedArrivalSeconds; }
        }

        public PathStopViewItem WithDistance(double distanceMeters)
        {
            return new PathStopViewItem(this.StopID, this.StopCode, this.Name,
                distanceMeters,
                this.PlannedArrivalSeconds);
        }
    }
}
