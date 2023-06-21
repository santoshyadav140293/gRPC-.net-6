using System;

namespace GrpcService1.DAL
{
    public class MarketWatchData
    {
        public string Symbol { get; set; }
        public double Price { get; set; }
        public double Volume { get; set; }
    }


    public class MboMbpBroadcast
    {
        public string eventTime { get; set; }
        public int eventType { get; set; }
        public string symbol { get; set; }
        public string priceChangePer { get; set; }
        public string priceChange { get; set; }
        public string weightedAveragePrice { get; set; }
        public string firstTrade { get; set; }

        public string closeTime { get; set; }

        public string quoteVolume { get; set; }

        public string bidQuantity { get; set; }

        public string bidPrice { get; set; }

        public string askQuantity { get; set; }

        public string askPrice { get; set; }
        
        public string openTime { get; set; }

        public string highPrice { get; set; }

        public string lastTradeId { get; set; }

        public string baseVolume { get; set; }

        public string lastQuantity { get; set; }

        public string lastPrice { get; set; }

        public int openPrice { get; set; }

        public int firstTradeId { get; set; }

        public int lowPrice { get; set; }

        public int totalTrades { get; set; }
    }
}
