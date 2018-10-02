using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.OrderbooksBridge.Settings.JobSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }

        [AzureBlobCheck]
        public string FailureBlobStorage { get; set; }

        [SqlCheck]
        public string SqlDbConnectionString { get; set; }
    }
}
