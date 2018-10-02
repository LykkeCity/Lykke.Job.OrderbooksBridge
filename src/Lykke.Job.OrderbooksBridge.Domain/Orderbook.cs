using System;
using System.Collections.Generic;

namespace Lykke.Job.OrderbooksBridge.Domain
{
    public class Orderbook
    {
        public long Id { get; set; }

        public string AssetPair { get; set; }

        public bool IsBuy { get; set; }

        public DateTime Timestamp { get; set; }

        public List<VolumePrice> Prices { get; set; }
    }

    public class VolumePrice
    {
        public double Volume { get; set; }

        public double Price { get; set; }
    }
}
