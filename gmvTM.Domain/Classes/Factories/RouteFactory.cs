using gmvTM.Domain.Items;

namespace gmvTM.Domain
{
    public static class RouteFactory
    {
        public static RouteItem CreateItem(string shortName, string longName, string? color, string encodedPolyline, int id = 0)
        {
            return new RouteItem
            {
                ID = id,
                ShortName = shortName,
                LongName = longName,
                Color = color,
                EncodedPolyline = encodedPolyline
            };
        }
    }
}
