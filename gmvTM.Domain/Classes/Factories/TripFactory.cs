using System;
using System.Collections.Generic;
using gmvTM.Domain.Items;
using gmvTM.Domain.Items.View;

namespace gmvTM.Domain
{
    public static class TripFactory
    {
        public static TripItem CreateItem(int routeID, int vehicleID, int startStopID, string status, DateTime startedAtUtc, double averageMph, int averageDwellSeconds)
        {
            return new TripItem
            {
                RouteID = routeID,
                VehicleID = vehicleID,
                StartStopID = startStopID,
                Status = status,
                StartedAtUtc = startedAtUtc,
                AverageMph = averageMph,
                AverageDwellSeconds = averageDwellSeconds
            };
        }

        public static TripItem CreateItem(int routeID, int vehicleID, IReadOnlyList<PathStopViewItem> stopsInOrder, int startIndex, string status, DateTime startedAtUtc, double averageMph, int averageDwellSeconds)
        {
            TripItem trip = CreateItem(routeID, vehicleID, stopsInOrder[startIndex].StopID, status, startedAtUtc, averageMph, averageDwellSeconds);

            foreach (StopTripItem stopTrip in StopTripFactory.CreateItems(stopsInOrder, startIndex, averageMph, averageDwellSeconds))
            {
                trip.StopTrips.Add(stopTrip);
            }

            return trip;
        }
    }
}
