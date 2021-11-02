using System;

namespace Stocks.General.Models
{
    public class TimestampPrices
    {
        public DateTimeOffset Date { get; set; }
        public decimal Price { get; set; }
        public bool Prediction { get; set; }
    }
}
