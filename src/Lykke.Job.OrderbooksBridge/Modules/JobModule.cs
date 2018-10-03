using Autofac;
using AzureStorage.Blob;
using Common;
using Lykke.Common.Log;
using Lykke.Job.OrderbooksBridge.Domain;
using Lykke.Job.OrderbooksBridge.Services;
using Lykke.Job.OrderbooksBridge.Settings.JobSettings;
using Lykke.Sdk;
using Lykke.Sdk.Health;
using Lykke.Job.OrderbooksBridge.RabbitSubscribers;
using Lykke.Job.OrderbooksBridge.Domain.Services;
using Lykke.Job.OrderbooksBridge.Settings;
using Lykke.Job.OrderbooksBridge.Sql;
using Lykke.Service.DataBridge.Data;
using Lykke.Service.DataBridge.Data.Abstractions;
using Lykke.SettingsReader;

namespace Lykke.Job.OrderbooksBridge.Modules
{
    public class JobModule : Module
    {
        private readonly OrderbooksBridgeJobSettings _settings;
        private readonly IReloadingManager<AppSettings> _settingsManager;

        public JobModule(OrderbooksBridgeJobSettings settings, IReloadingManager<AppSettings> settingsManager)
        {
            _settings = settings;
            _settingsManager = settingsManager;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .SingleInstance();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>()
                .AutoActivate()
                .SingleInstance();

            builder.RegisterType<RabbitSubscriber>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _settings.Rabbit.ConnectionString)
                .WithParameter("exchangeName", _settings.Rabbit.ExchangeName);

            builder.Register(ctx =>
                {
                    var logFactory = ctx.Resolve<ILogFactory>();
                    var blobStorage = AzureBlobStorage.Create(_settingsManager.ConnectionString(i => i.OrderbooksBridgeJob.Db.FailureBlobStorage));
                    var repository = new DataRepository<Orderbook, DataContext>(
                        _settings.Db.SqlDbConnectionString,
                        _settings.MaxBatchCount,
                        blobStorage,
                        _settings.BlobStorageCheckPeriod,
                        _settings.WarningPeriod,
                        _settings.WarningSqlTableSizeInGigabytes,
                        _settings.BatchPeriod,
                        logFactory,
                        new DbContextExtFactory(),
                        new DbEntityMapper()/*,
                        new DbOrderbooksProcessor()*/ //not using it cause db items are fetched for search too slow without indexes
                        );
                    return repository;
                })
                .AsSelf()
                .As<IStartable>()
                .As<IStopable>()
                .As<IDataRepository>()
                .SingleInstance();
        }
    }
}
