using System.Collections.Generic;

namespace Stocks.General.Models
{
    public class AllStocksHistoricalPricesDaily
    {
        public string Ticker { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UrlLogo { get; set; }
        public IList<OHLCPriceValue> HistoricalPrices { get; set; } = new List<OHLCPriceValue>();
    }
}
