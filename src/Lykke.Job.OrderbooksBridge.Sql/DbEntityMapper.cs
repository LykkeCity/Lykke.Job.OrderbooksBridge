using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Job.OrderbooksBridge.Domain;
using Lykke.Job.OrderbooksBridge.Sql.Models;
using Lykke.Service.DataBridge.Data.Abstractions;

namespace Lykke.Job.OrderbooksBridge.Sql
{
    public class DbEntityMapper : IDbEntityMapper
    {
        public Task<List<object>> MapEntityAsync(object item)
        {
            var result = new List<object>();
            switch (item)
            {
                case Orderbook ob:
                    result.Add(OrderBookForSqlDb.FromModel(ob));
                    break;
                default:
                    result.Add(item);
                    break;
            }
            return Task.FromResult(result);
        }
    }
}
