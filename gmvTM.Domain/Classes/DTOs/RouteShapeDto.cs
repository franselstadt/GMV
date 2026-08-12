using System.Collections.Generic;
using gmvTM.Domain.Items.View;

namespace gmvTM.Domain
{
    public class RouteShapeDto
    {
        public List<CoordinatesViewItem> Points { get; set; } = new List<CoordinatesViewItem>();
    }
}
