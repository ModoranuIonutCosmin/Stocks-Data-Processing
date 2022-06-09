using Stocks.General.Models.StocksInfoAggregates;
using System;

namespace Stocks.General.Models.TradeSuggestions
{
    public class TradeSuggestion
    {
        public string Ticker { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal ExpectedPrice { get; set; }
        public PlaceMarketOrderRequest OpenRequest { get; set; }
    }
}
