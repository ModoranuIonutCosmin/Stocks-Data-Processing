using Stocks.General.Models;
using StocksProccesing.Relational.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator
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

        public decimal CalculateTrendFromOHLC(OHLCPriceValue stocksOHLC)
        {
            return 100 * (stocksOHLC.CloseValue / stocksOHLC.OpenValue - 1);
        }

        public decimal CalculateTrendFromOHLC(StocksOHLC stocksOHLC)
        {
            return 100 * (stocksOHLC.CloseValue / stocksOHLC.OpenValue - 1);
        }
    }
}
