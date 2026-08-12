namespace gmvTM.Domain
{
    public static class gmvDomain
    {
        private static readonly Messages messages = new Messages();
        private static readonly Tables tables = new Tables();
        private static readonly Columns columns = new Columns();
        private static readonly ScheduleStatuses scheduleStatuses = new ScheduleStatuses();
        private static readonly AppConstants appConstants = new AppConstants();
        private static readonly Resources resources = new Resources();
        private static readonly VehiclePhases vehiclePhases = new VehiclePhases();

        public static Messages Messages => messages;
        public static Tables Tables => tables;
        public static Columns Columns => columns;
        public static ScheduleStatuses ScheduleStatuses => scheduleStatuses;
        public static AppConstants AppConstants => appConstants;
        public static Resources Resources => resources;
        public static VehiclePhases VehiclePhases => vehiclePhases;
    }
}
