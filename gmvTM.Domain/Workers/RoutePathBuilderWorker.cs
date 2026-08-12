using System;
using System.Collections.Generic;
using gmvTM.Domain.Items.View;
using gmvTM.Domain.Workers.Interfaces;

namespace gmvTM.Domain.Workers
{
    public sealed class RoutePathBuilderWorker : IRoutePathBuilderWorker
    {
        //copied from claude helped here
        public RoutePathViewItem Build(IReadOnlyList<CoordinatesViewItem> decodedPoints)
        {
            ArgumentNullException.ThrowIfNull(decodedPoints);

            if (decodedPoints.Count < 2)
                throw new ArgumentException(gmvDomain.Messages.RoutePathNeedsTwoPoints, nameof(decodedPoints));

            List<CoordinatesViewItem> points = new List<CoordinatesViewItem>(decodedPoints);
            List<double> cumulative = new List<double>(points.Count) { 0 };

            for (int i = 1; i < points.Count; i++)
            {
                double segment = RoutePathViewItem.HaversineMeters(points[i - 1], points[i]);
                cumulative.Add(cumulative[i - 1] + segment);
            }

            return new RoutePathViewItem(points, cumulative);
        }
    }
}
