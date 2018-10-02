using System;

namespace Lykke.Job.OrderbooksBridge.Settings.JobSettings
{
    public class OrderbooksBridgeJobSettings
    {
        public DbSettings Db { get; set; }
        public RabbitMqSettings Rabbit { get; set; }
        public int MaxBatchCount { get; set; }
        public TimeSpan BatchPeriod { get; set; }
        public TimeSpan BlobStorageCheckPeriod { get; set; }
        public TimeSpan WarningPeriod { get; set; }
        public int WarningSqlTableSizeInGigabytes { get; set; }
    }
}
