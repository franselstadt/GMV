using gmvTM.Domain.Collections.Base;
using gmvTM.Domain.Items.View;

namespace gmvTM.Domain.Collections.View
{
    public sealed class CoordinatesViewCollection : ViewCollection<CoordinatesViewItem>
    {
        public CoordinatesViewCollection()
        {
        }

        public CoordinatesViewCollection(System.Collections.Generic.IEnumerable<CoordinatesViewItem> items) : base(items)
        {
        }
    }
}
