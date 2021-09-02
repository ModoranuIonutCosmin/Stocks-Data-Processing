using System;
using System.Collections.Generic;
using System.Linq;
using Stocks.General.ExtensionMethods;

namespace StocksProcessing.API.Models
{
    public class WholeStocksPricePredictionsModel
    {
        public string Ticker { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UrlLogo { get; set; }
        public double Trend { get {

                if(Predictions.Count <= 0)
                {
                    return 0;
                }

                var currentDayStart = DateTimeOffset.UtcNow.SetTime(8, 0);
                var currentDayEnd = DateTimeOffset.UtcNow.SetTime(23, 59);
                double? todaysOpenPrice = new double();
                double? todaysLatestClosePrice = new double();

                for (int i = Predictions.Count - 1; i >= 0; i--)
                {
                    if(Predictions[i].TimeStamp < currentDayStart)
                    {
                        if(i + 1 < Predictions.Count)
                        {
                            todaysOpenPrice = Predictions[i + 1].Price;
                            break;
                        }
                    }
                }

                todaysLatestClosePrice = Predictions.Where(e => e.TimeStamp < currentDayEnd)
                    .Last().Price;

                if (!todaysOpenPrice.HasValue || !todaysLatestClosePrice.HasValue)
                    return 0;


                return
                    Math.Round(100 -
                    (todaysOpenPrice.Value / todaysLatestClosePrice.Value) * 100, 2);
            } }
        public IList<TimestampPrices> Predictions { get; set; } = new List<TimestampPrices>();
    }
}
