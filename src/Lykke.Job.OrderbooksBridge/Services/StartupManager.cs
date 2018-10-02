using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.OrderbooksBridge.Domain.Services;
using Lykke.Sdk;

namespace Lykke.Job.OrderbooksBridge.Services
{
    public class StartupManager : IStartupManager
    {
        private readonly ILog _log;
        private readonly List<IStartable> _startables = new List<IStartable>();

        public StartupManager(ILogFactory logFactory, IEnumerable<IStartStop> startables)
        {
            _log = logFactory.CreateLog(this);
            _startables.AddRange(startables);
        }

        public Task StartAsync()
        {
            foreach (var startable in _startables)
            {
                startable.Start();
            }

            return Task.CompletedTask;
        }
    }
}
