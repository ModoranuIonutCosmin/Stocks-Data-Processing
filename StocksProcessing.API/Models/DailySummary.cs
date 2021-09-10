using System;

namespace StocksProcessing.API.Models
{
    public class DailySummary
    {
        public string Ticker { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double OpenValue { get; set; }
        public double CloseValue { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
