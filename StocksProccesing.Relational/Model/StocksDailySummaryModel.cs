using System;

namespace StocksProcessing.API.Models
{
    public class StocksDailySummaryModel
    {
        public string Ticker { get; set; }

        public string Name { get; set; }
        public string UrlLogo { get; set; }

        public double Trend { get { return Math.Round(100 - CloseValue / OpenValue * 100, 2); } }
        public double High { get; set; }
        public double Low { get; set; }
        public double OpenValue { get; set; }
        public double CloseValue { get; set; }
    }
}
