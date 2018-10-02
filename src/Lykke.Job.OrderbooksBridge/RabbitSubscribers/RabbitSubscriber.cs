using System;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Job.OrderbooksBridge.Domain;
using Lykke.Job.OrderbooksBridge.Domain.Services;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.DataBridge.Data.Abstractions;

namespace Lykke.Job.OrderbooksBridge.RabbitSubscribers
{
    public class RabbitSubscriber : IStartStop
    {
        private readonly ILogFactory _logFactory;
        private readonly IDataRepository _dataRepository;
        private readonly string _connectionString;
        private readonly string _exchangeName;

        private RabbitMqSubscriber<Orderbook> _subscriber;

        public RabbitSubscriber(
            ILogFactory logFactory,
            IDataRepository dataRepository,
            string connectionString,
            string exchangeName)
        {
            _logFactory = logFactory;
            _dataRepository = dataRepository;
            _connectionString = connectionString;
            _exchangeName = exchangeName;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .ForSubscriber(_connectionString, _exchangeName, "orderbooksbridgejob")
                .MakeDurable();

            _subscriber = new RabbitMqSubscriber<Orderbook>(
                    _logFactory,
                    settings,
                    new ResilientErrorHandlingStrategy(
                        _logFactory,
                        settings,
                        TimeSpan.FromSeconds(10),
                        next: new DeadQueueErrorHandlingStrategy(_logFactory, settings)))
                .SetMessageDeserializer(new JsonMessageDeserializer<Orderbook>())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .Start();
        }

        private async Task ProcessMessageAsync(Orderbook arg)
        {
            await _dataRepository.AddDataItemAsync(arg);
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }

        public void Stop()
        {
            _subscriber?.Stop();
        }
    }
}
