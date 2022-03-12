using System;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;

namespace StocksFinalSolution.BusinessLogic.Features.StocksMarketMetricsCalculator
{
    public class PricesDisparitySimulator : IPricesDisparitySimulator
    {
        public decimal ComputeSpreadAmount()
        {
            var currentDate = DateTimeOffset.UtcNow;
            var dayNumeral = currentDate.Day;

            return dayNumeral * 0.00019m;
        }
    }
}
