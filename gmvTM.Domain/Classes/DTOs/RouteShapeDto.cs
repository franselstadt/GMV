using System.Collections.Generic;

namespace gmvTM.Domain
{
    public class Point
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class RouteShapeDto
    {
        public List<Point> Points { get; set; }
    }
}
