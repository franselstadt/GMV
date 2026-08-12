using System;
using gmvTM.Domain.Items.Base;

namespace gmvTM.Domain.Items.View
{
    public sealed class CoordinatesViewItem : ViewItem
    {
        private readonly double latitude;
        private readonly double longitude;

        public CoordinatesViewItem(double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90)
            {
                throw new ArgumentOutOfRangeException(nameof(latitude), latitude, Messages.LatitudeOutOfRange);
            }

            if (longitude < -180 || longitude > 180)
            {
                throw new ArgumentOutOfRangeException(nameof(longitude), longitude, Messages.LongitudeOutOfRange);
            }

            this.latitude = latitude;
            this.longitude = longitude;
        }

        public override string ViewName
        {
            get { return "Coordinates"; }
        }

        public double Latitude
        {
            get { return this.latitude; }
        }

        public double Longitude
        {
            get { return this.longitude; }
        }
    }
}
