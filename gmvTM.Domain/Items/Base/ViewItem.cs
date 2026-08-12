using System.Text.Json.Serialization;
using gmvTM.Domain.Items.Interfaces;

namespace gmvTM.Domain.Items.Base
{
    public abstract class ViewItem : IViewItem
    {
        [JsonIgnore]
        public abstract string ViewName
        {
            get;
        }
    }
}
