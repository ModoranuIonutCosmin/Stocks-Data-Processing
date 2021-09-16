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
        public double Trend { get { return Math.Round(100 - CloseValue / OpenValue * 100, 2); } }
        public double High { get; set; }
        public double Low { get; set; }
        public double OpenValue { get; set; }
        public double CloseValue { get; set; }
        public double SellPrice { get => CloseValue; }
        public double BuyPrice { get => (SellPrice + SellPrice * TaxesConfig.FullSpreadFees).TruncateToDecimalPlaces(3); }
    }
}
