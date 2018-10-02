using Autofac;
using Common;

namespace Lykke.Job.OrderbooksBridge.Domain.Services
{
    public interface IStartStop : IStartable, IStopable
    {
    }
}
