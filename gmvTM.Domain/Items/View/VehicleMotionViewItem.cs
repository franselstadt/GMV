using System;
using gmvTM.Domain.Items.Base;

namespace gmvTM.Domain.Items.View
{
    public sealed class VehicleMotionViewItem : ViewItem
    {
        private readonly CoordinatesViewItem position;
        private readonly string phase;
        private readonly string stopCode;
        private readonly string stopName;
        private readonly double? secondsToStop;

        public VehicleMotionViewItem(CoordinatesViewItem position, string phase, string stopCode, string stopName, double? secondsToStop)
        {
            ArgumentNullException.ThrowIfNull(position);
            ArgumentException.ThrowIfNullOrWhiteSpace(phase);

            this.position = position;
            this.phase = phase;
            this.stopCode = stopCode;
            this.stopName = stopName;
            this.secondsToStop = secondsToStop;
        }

        public override string ViewName
        {
            get { return "VehicleMotion"; }
        }

        public CoordinatesViewItem Position
        {
            get { return this.position; }
        }

        public string Phase
        {
            get { return this.phase; }
        }

        public string StopCode
        {
            get { return this.stopCode; }
        }

        public string StopName
        {
            get { return this.stopName; }
        }

        public double? SecondsToStop
        {
            get { return this.secondsToStop; }
        }
    }
}
