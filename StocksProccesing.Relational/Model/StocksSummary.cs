using Stocks.General.Models;
using System.Collections.Generic;

namespace StocksProcessing.API.Models
{
    public class StocksSummary
    {
        public string Ticker { get; set; }
        public string Name { get; set; }
        public string UrlLogo { get; set; }

        public string Description { get; set; }
        public decimal Trend { get; set; }
        public decimal SellPrice { get; set; }
        public decimal BuyPrice { get; set; }
        public long Period { get; set; }
        public List<OHLCPriceValue> Timepoints { get; set; } = new List<OHLCPriceValue>();
    }
}
