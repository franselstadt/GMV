using System.Collections.Generic;
using gmvTM.Domain.Items.View;

namespace gmvTM.Domain.Workers.Interfaces
{
    public interface IPolylineDecoderWorker : IWorker
    {
        public List<CoordinatesViewItem> Decode(string encodedPolyline);
    }
}
