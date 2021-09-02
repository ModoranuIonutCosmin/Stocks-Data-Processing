using System;

namespace StocksProcessing.API.Models
{
    public class TimestampPrices
    {
        public DateTimeOffset TimeStamp { get; set; }
        public double Price { get; set; }
        public bool Prediction { get; set; }
    }
}
