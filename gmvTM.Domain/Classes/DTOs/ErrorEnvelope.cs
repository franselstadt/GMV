using System.Collections.Generic;

namespace gmvTM.Domain
{
    public class ErrorEnvelope
    {
        public string TraceID { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public List<string> Messages { get; set; }
    }
}
