using Stocks.General.Models.StocksInfoAggregates;

namespace Stocks.General.Models
{
    public class StockReportSingle
    {
        public string Ticker { get; set; }
        public string Name { get; set; }
        public string UrlLogo { get; set; }
        public string Description { get; set; }
        public decimal Trend { get; set; }
        public decimal SellPrice { get; set; }
        public decimal BuyPrice { get; set; }
        public long Period { get; set; }
        public OhlcPriceValue Timepoint { get; set; }
    }
}
