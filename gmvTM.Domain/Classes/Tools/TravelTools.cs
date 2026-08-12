using System;

namespace gmvTM.Domain.Classes.Tools
{
    public static class TravelTools
    {
        public static double MetersPerSecondFromMph(double mph) => mph * gmvDomain.AppConstants.MetersPerMile / gmvDomain.AppConstants.SecondsPerHour;

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
