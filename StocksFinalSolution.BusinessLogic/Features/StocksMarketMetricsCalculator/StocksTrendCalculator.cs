using System;
using System.Collections.Generic;
using System.Linq;
using Stocks.General.Models.StocksInfoAggregates;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Features.StocksMarketMetricsCalculator
{
    public class StocksTrendCalculator : IStocksTrendCalculator
    {
        public decimal CalculateTrendFromList(List<StocksPriceData> lastRangePricesData)
        {
            if (lastRangePricesData is null || lastRangePricesData.Count == 0)
            {
                throw new ArgumentException(nameof(lastRangePricesData));
            }

            if(lastRangePricesData.Last().Price == 0)
            {
                throw new InvalidOperationException("On trend calculation, current price would " +
                    "have been 0 before division");
            }

            return 100 * (lastRangePricesData.Last().Price / lastRangePricesData.First().Price - 1);
        }

        public decimal CalculateTrendFromOHLC(OhlcPriceValue stocksOHLC)
        {
            return 100 * (stocksOHLC.CloseValue / stocksOHLC.OpenValue - 1);
        }

        public decimal CalculateTrendFromOHLC(StocksOhlc stocksOHLC)
        {
            return 100 * (stocksOHLC.CloseValue / stocksOHLC.OpenValue - 1);
        }
    }
}
