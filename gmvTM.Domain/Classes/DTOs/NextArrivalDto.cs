using System;

using gmvTM.Domain.Classes.DTOs.Base;

namespace gmvTM.Domain
{
    public class NextArrivalDto : BaseDTO
    {
        public string StopCode { get; set; }
        public string RunLabel { get; set; }
        public TimeOnly PlannedTime { get; set; }
        public TimeOnly? ActualTime { get; set; }
        public string Status { get; set; }
    }
}
