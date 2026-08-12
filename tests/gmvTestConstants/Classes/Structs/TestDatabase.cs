using System;
using System.IO;

namespace gmvTestConstants
{
    public readonly struct TestDatabase
    {
        public string SharedMemorySuffix => "-shm";
        public string WriteAheadLogSuffix => "-wal";

        public string NewDatabasePath()
        {
            return Path.Combine(Path.GetTempPath(), $"gmvtm-tests-{Guid.NewGuid():N}.db");
        }

        public string ConnectionString(string databasePath)
        {
            return $"Data Source={databasePath}";
        }
    }
}
