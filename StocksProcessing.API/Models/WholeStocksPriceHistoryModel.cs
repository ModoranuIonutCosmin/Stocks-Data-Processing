using System;
using System.Collections.Generic;
using System.Linq;
using Stocks.General.ExtensionMethods;

namespace StocksProcessing.API.Models
{
    public class WholeStocksPriceHistoryModel
    {
        public string Ticker { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UrlLogo { get; set; }
        public double Trend { get {

                if(HistoricalPrices.Count <= 0)
                {
                    return 0;
                }

                var currentDayStart = DateTimeOffset.UtcNow.SetTime(8, 0);
                double? todaysOpenPrice = new double();
                double todaysLatestClosePrice;

                for (int i = HistoricalPrices.Count - 1; i >= 0; i--)
                {
                    if(HistoricalPrices[i].TimeStamp < currentDayStart)
                    {
                        if(i + 1 < HistoricalPrices.Count)
                        {
                            todaysOpenPrice = HistoricalPrices[i + 1].Price;
                            break;
                        }
                    }
                }

                if (!todaysOpenPrice.HasValue)
                    return 0;

                todaysLatestClosePrice = HistoricalPrices.Last().Price;

                return
                    Math.Round(100 -
                    (todaysOpenPrice.Value / todaysLatestClosePrice) * 100, 2);
            } }
        public IList<TimestampPrices> HistoricalPrices { get; set; } = new List<TimestampPrices>();
    }
}
