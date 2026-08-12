using gmvTM.Domain.Collections.Base;
using gmvTM.Domain.Items.View;

namespace gmvTM.Domain.Collections.View
{
    public sealed class PathStopViewCollection : ViewCollection<PathStopViewItem>
    {
        public PathStopViewCollection()
        {
        }

        public PathStopViewCollection(System.Collections.Generic.IEnumerable<PathStopViewItem> items) : base(items)
        {
        }
    }
}
