using System;
using System.Collections.Generic;
using gmvTM.Domain.Items.Base;

namespace gmvTM.Domain.Items.View
{
    public sealed class TripPathViewItem : ViewItem
    {
        private readonly RoutePathViewItem path;
        private readonly IReadOnlyList<PathStopViewItem> stops;

        public TripPathViewItem(RoutePathViewItem path, IReadOnlyList<PathStopViewItem> stops)
        {
            this.path = path ?? throw new ArgumentNullException(nameof(path));
            this.stops = stops ?? throw new ArgumentNullException(nameof(stops));
        }

        public override string ViewName
        {
            get { return "TripPath"; }
        }

        public RoutePathViewItem Path
        {
            get { return this.path; }
        }

        public IReadOnlyList<PathStopViewItem> Stops
        {
            get { return this.stops; }
        }
    }
}
