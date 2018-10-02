using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Dapper;
using Lykke.Job.OrderbooksBridge.Sql.Models;
using Lykke.Service.DataBridge.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Job.OrderbooksBridge.Sql
{
    public class DbOrderbooksProcessor : INotIdentifiableItemsProcessor
    {
        private const string _format = "yyyy-MM-dd HH:mm:ss";

        private readonly Dictionary<string, List<OrderBookForSqlDb>> _cache;

        private static DateTime? _cacheDate;

        public async Task<object> FindCopyInDbAsync(object newObject, DbContextExt context)
        {
            var item = newObject as OrderBookForSqlDb;

            if (_cache == null
                || !_cacheDate.HasValue
                || item.Timestamp.Hour != _cacheDate.Value.Hour
                || !_cache.ContainsKey(item.AssetPair))
                await FillCacheAsync(item, context);

            if (!_cache.ContainsKey(item.AssetPair))
                return null;

            var fromDb = _cache[item.AssetPair].FirstOrDefault(c => c.IsBuy == item.IsBuy && c.Timestamp == item.Timestamp);
            return fromDb;
        }

        public async Task AddToContextAsync(object item, DbContextExt context)
        {
            if (context is DataContext dbContext && item is OrderBookForSqlDb newObject)
                await dbContext.Orderbooks.AddAsync(newObject);
            else
                await context.AddAsync(item);
        }

        public bool UpdateInContext(object oldVersion, object newVersion, DbContextExt context)
        {
            if (!(oldVersion is OrderBookForSqlDb oldItem) || !(newVersion is OrderBookForSqlDb newItem))
                return false;

            if (oldItem.BestPrice == newItem.BestPrice)
                return false;

            oldItem.BestPrice = newItem.BestPrice;
            return true;
        }

        private async Task FillCacheAsync(OrderBookForSqlDb item, DbContextExt context)
        {
            DateTime from = item.Timestamp.RoundToHour();
            DateTime to = from.AddHours(1);
            string query = $"SELECT * FROM dbo.{DataContext.OrderbooksTable} WHERE AssetPair = '{item.AssetPair}' AND Timestamp >= '{from.ToString(_format)}' AND Timestamp < '{to.ToString(_format)}'";
            var queryResult = await context.Database.GetDbConnection().QueryAsync<OrderBookForSqlDb>(new CommandDefinition(query, commandTimeout: 900));
            _cacheDate = from;
            _cache[item.AssetPair] = queryResult.ToList();
        }
    }
}
