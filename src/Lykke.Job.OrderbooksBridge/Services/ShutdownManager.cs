using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.OrderbooksBridge.Domain.Services;
using Lykke.Sdk;

namespace Lykke.Job.OrderbooksBridge.Services
{
    public class ShutdownManager : IShutdownManager
    {
        private readonly ILog _log;
        private readonly List<IStopable> _items = new List<IStopable>();
        private readonly List<IStopable> _stopables = new List<IStopable>();

        public ShutdownManager(
            ILogFactory logFactory,
            IEnumerable<IStartStop> items,
            IEnumerable<IStopable> stopables)
        {
            _log = logFactory.CreateLog(this);
            _items.AddRange(items);
            _stopables.AddRange(stopables);
        }

        public async Task StopAsync()
        {
            Parallel.ForEach(_items, i =>
            {
                try
                {
                    i.Stop();
                }
                catch (Exception ex)
                {
                    _log.Warning($"Unable to stop {i.GetType().Name}", ex);
                }
            });

            Parallel.ForEach(_stopables, i =>
            {
                try
                {
                    i.Stop();
                }
                catch (Exception ex)
                {
                    _log.Warning($"Unable to stop {i.GetType().Name}", ex);
                }
            });

            await Task.CompletedTask;
        }
    }
}
