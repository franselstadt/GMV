using System;

namespace gmvTM.Domain
{
    public class NextArrivalDto
    {
        public string StopCode { get; set; }
        public string RunLabel { get; set; }
        public TimeOnly PlannedTime { get; set; }
        public TimeOnly? ActualTime { get; set; }
        public string Status { get; set; }
    }
}
