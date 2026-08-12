using System.Collections.Generic;
using gmvTM.Domain.Items.View;

using gmvTM.Domain.Classes.DTOs.Base;

namespace gmvTM.Domain
{
    public class RouteShapeDto : BaseDTO
    {
        public List<CoordinatesViewItem> Points { get; set; } = new List<CoordinatesViewItem>();
    }
}
