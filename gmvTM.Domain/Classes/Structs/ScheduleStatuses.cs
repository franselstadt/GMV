namespace gmvTM.Domain
{
    public readonly struct ScheduleStatuses
    {
        public string OnTime => "OnTime";
        public string RunningLate => "RunningLate";
        public string Ahead => "Ahead";
    }
}
