using Stocks.General.Models;
using StocksProccesing.Relational.Model;
using System.Collections.Generic;

namespace StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator
{
    public interface IStocksTrendCalculator
    {
        decimal CalculateTrendFromList(List<StocksPriceData> lastRangePricesData);
        decimal CalculateTrendFromOHLC(OhlcPriceValue stocksOHLC);
        decimal CalculateTrendFromOHLC(StocksOhlc stocksOHLC);

    }
}