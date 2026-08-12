using gmvTM.Domain.Items.Interfaces;

namespace gmvTM.Domain.Items.Base
{
    public abstract class ViewItem : IViewItem
    {
        public abstract string ViewName
        {
            get;
        }
    }
}
