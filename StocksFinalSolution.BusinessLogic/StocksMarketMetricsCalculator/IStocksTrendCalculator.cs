using StocksProccesing.Relational.Model;
using System.Collections.Generic;

namespace StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator
{
    public interface IStocksTrendCalculator
    {
        decimal CalculateTrend(List<StocksPriceData> lastRangePricesData);
    }
}