using System.Collections.Generic;

using gmvTM.Domain.Classes.DTOs.Base;

namespace gmvTM.Domain
{
    public class ErrorEnvelope : BaseDTO
    {
        public string TraceID { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public List<string> Messages { get; set; }
    }
}
