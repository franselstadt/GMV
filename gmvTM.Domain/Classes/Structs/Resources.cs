namespace gmvTM.Domain
{
    public readonly struct Resources
    {
        public string DefaultConnectionStringName => "Default";
        public string DefaultSqliteConnection => "Data Source=gmvtm.db";
        public string ConnectionStringsDefaultSetting => "ConnectionStrings:Default";
        public string SeedFileName => "route-f-seed.json";
        public string SeedOutputFolder => "Seeding";
        public string SampleFleetCode => "LADOT-2201";
        public string SampleVehicleMake => "New Flyer";
        public string SampleVehicleModel => "XE40";
        public string SampleLicensePlate => "8LAD2201";
        public int SampleVehicleCapacity => 40;
        public int SampleVehicleModelYear => 2022;
    }
}
