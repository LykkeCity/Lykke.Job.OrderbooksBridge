using System;
using System.Linq;
using Lykke.Job.OrderbooksBridge.Domain;
using Lykke.Service.DataBridge.Data.Abstractions;

namespace Lykke.Job.OrderbooksBridge.Sql.Models
{
    public class OrderBookForSqlDb : DbIdentityBase
    {
        public override object DbId => null;

        public long Id { get; set; }

        public string AssetPair { get; set; }

        public bool IsBuy { get; set; }

        public DateTime Timestamp { get; set; }

        public double BestPrice { get; set; }

        public static OrderBookForSqlDb FromModel(Orderbook model)
        {
            double bestPrice = 0;
            if (model.Prices != null && model.Prices.Count > 0)
                bestPrice = model.IsBuy
                    ? model.Prices.Max(p => p.Price)
                    : model.Prices.Min(p => p.Price);

            return new OrderBookForSqlDb
            {
                AssetPair = model.AssetPair,
                IsBuy = model.IsBuy,
                Timestamp = model.Timestamp,
                BestPrice = bestPrice,
            };
        }

        public override bool Update(object newVersion)
        {
            var item = newVersion as OrderBookForSqlDb;
            if (item == null)
                return false;

            bool changed = BestPrice != item.BestPrice
                || Timestamp != item.Timestamp
                || IsBuy != item.IsBuy
                || AssetPair != item.AssetPair;
            BestPrice = item.BestPrice;
            Timestamp = item.Timestamp;
            IsBuy = item.IsBuy;
            AssetPair = item.AssetPair;
            return changed;
        }
    }
}
