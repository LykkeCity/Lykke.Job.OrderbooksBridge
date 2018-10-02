using Lykke.Job.OrderbooksBridge.Settings.JobSettings;
using Lykke.Sdk.Settings;

namespace Lykke.Job.OrderbooksBridge.Settings
{
    public class AppSettings : BaseAppSettings
    {
        public OrderbooksBridgeJobSettings OrderbooksBridgeJob { get; set; }
    }
}
