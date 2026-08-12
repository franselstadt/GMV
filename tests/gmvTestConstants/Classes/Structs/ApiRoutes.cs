namespace gmvTestConstants
{
    public readonly struct ApiRoutes
    {
        public string StopsForRouteF => "/api/v1/routes/f/stops";
        public string StopsForUnknownRoute => "/api/v1/routes/zzz/stops";
        public string NextArrivalsForStop6138 => "/api/v1/routes/f/stops/6138/arrivals/next";
        public string SimulationsForRouteF => "/api/v1/routes/f/simulations";
        public string DatabaseReseed => "/api/v1/admin/database/reseed";
        public string ODataStopItems => "/odata/StopItems";
        public string ODataRouteItemsFilteredToF => "/odata/RouteItems?$filter=ShortName eq 'F'&$select=ShortName,LongName";
    }
}
