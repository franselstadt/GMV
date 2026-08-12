namespace gmvTM.Domain
{
    public readonly struct VehiclePhases
    {
        public string Traveling => "traveling";
        public string Approaching => "approaching";
        public string DoorsOpen => "doorsOpen";
        public string DoorsClosing => "doorsClosing";
        public string Completed => "completed";
    }
}
