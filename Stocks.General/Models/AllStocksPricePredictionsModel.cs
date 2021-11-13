using System.Collections.Generic;

namespace Stocks.General.Models
{
    public class AllStocksPricePredictionsModel
    {
        public string Ticker { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UrlLogo { get; set; }
        public IList<TimestampPrices> Predictions { get; set; } = new List<TimestampPrices>();
    }
}
