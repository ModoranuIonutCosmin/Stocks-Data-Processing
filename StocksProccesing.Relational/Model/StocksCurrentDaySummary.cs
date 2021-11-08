using Stocks.General.ConstantsConfig;
using Stocks.General.ExtensionMethods;
using System;

namespace StocksProcessing.API.Models
{
    public class StocksCurrentDaySummary
    {
        public string Ticker { get; set; }
        public string Name { get; set; }
        public string UrlLogo { get; set; }
        public decimal Trend { get { return Math.Round(CloseValue / OpenValue * 100 - 100, 2); } }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal OpenValue { get; set; }
        public decimal CloseValue { get; set; }
        public decimal SellPrice { get => CloseValue; }
        public decimal BuyPrice { get => (SellPrice + SellPrice * TaxesConfig.FullSpreadFees).TruncateToDecimalPlaces(3); }
    }
}
