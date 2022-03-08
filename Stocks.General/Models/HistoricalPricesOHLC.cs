using System.Collections.Generic;

namespace Stocks.General.Models
{
    public class HistoricalPricesOhlc
    {
        public string Ticker { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UrlLogo { get; set; }
        public IList<OhlcPriceValue> HistoricalPrices { get; set; } = new List<OhlcPriceValue>();
    }
}
