using gmvTM.Domain.Items;

namespace gmvTM.Domain
{
    public static class StopFactory
    {
        public static StopItem CreateItem(int routeID, string stopCode, string name, double latitude, double longitude, int sequence, string? specialAlert = null, int id = 0)
        {
            return new StopItem
            {
                ID = id,
                RouteID = routeID,
                StopCode = stopCode,
                Name = name,
                Latitude = latitude,
                Longitude = longitude,
                Sequence = sequence,
                SpecialAlert = specialAlert
            };
        }
    }
}
