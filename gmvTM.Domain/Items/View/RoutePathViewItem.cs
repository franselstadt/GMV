using System;
using System.Linq;
using System.Collections.Generic;

namespace gmvTM.Domain.Items.View
{
    public sealed class RoutePathViewItem : Base.ViewItem
    {
        private readonly IReadOnlyList<CoordinatesViewItem> points;
        private readonly IReadOnlyList<double> cumulativeMeters;

        public RoutePathViewItem(IReadOnlyList<CoordinatesViewItem> points, IReadOnlyList<double> cumulativeMeters)
        {
            ArgumentNullException.ThrowIfNull(points);
            ArgumentNullException.ThrowIfNull(cumulativeMeters);

            if (points.Count < 2)
                throw new ArgumentException(gmvDomain.Messages.RoutePathNeedsTwoPoints, nameof(points));

            if (points.Count != cumulativeMeters.Count)
                throw new ArgumentException(gmvDomain.Messages.CumulativeDistancesMustMatchPoints);

            this.points = points;
            this.cumulativeMeters = cumulativeMeters;
        }

        public override string ViewName
        {
            get { return "RoutePath"; }
        }

        public IReadOnlyList<CoordinatesViewItem> Points
        {
            get { return this.points; }
        }

        public IReadOnlyList<double> CumulativeMeters
        {
            get { return this.cumulativeMeters; }
        }

        public double TotalMeters
        {
            get { return !this.CumulativeMeters.Any() ? 0 : this.CumulativeMeters[this.CumulativeMeters.Count - 1]; }
        }

        public CoordinatesViewItem PointAtDistance(double metersAlongPath)
        {
            double target = Math.Clamp(metersAlongPath, 0, this.TotalMeters);

            for (int i = 1; i < this.CumulativeMeters.Count; i++)
            {
                if (this.CumulativeMeters[i] < target)
                    continue;

                double segmentStart = this.CumulativeMeters[i - 1];
                double segmentLength = this.CumulativeMeters[i] - segmentStart;
                double t = segmentLength <= 0 ? 0 : (target - segmentStart) / segmentLength;
                CoordinatesViewItem a = this.Points[i - 1];
                CoordinatesViewItem b = this.Points[i];

                //revisit later
                return new CoordinatesViewItem(
                    a.Latitude + ((b.Latitude - a.Latitude) * t),
                    a.Longitude + ((b.Longitude - a.Longitude) * t));
            }

            return this.Points[this.Points.Count - 1];
        }

        public double NearestDistanceMeters(CoordinatesViewItem point)
        {
            ArgumentNullException.ThrowIfNull(point);

            double best = 0;
            double bestDistance = double.MaxValue;

            for (int i = 0; i < this.Points.Count; i++)
            {
                double d = HaversineMeters(point, this.Points[i]);
                if (d >= bestDistance)
                    continue;

                bestDistance = d;
                best = this.CumulativeMeters[i];
            }

            return best;
        }


        //unit test
        public static double HaversineMeters(CoordinatesViewItem a, CoordinatesViewItem b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);

            const double earthRadiusMeters = 6371000;
            double lat1 = DegreesToRadians(a.Latitude);
            double lat2 = DegreesToRadians(b.Latitude);
            double dLat = DegreesToRadians(b.Latitude - a.Latitude);
            double dLng = DegreesToRadians(b.Longitude - a.Longitude);

            double h = (Math.Sin(dLat / 2) * Math.Sin(dLat / 2))
                     + (Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(dLng / 2) * Math.Sin(dLng / 2));

            return 2 * earthRadiusMeters * Math.Asin(Math.Min(1, Math.Sqrt(h)));
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}
