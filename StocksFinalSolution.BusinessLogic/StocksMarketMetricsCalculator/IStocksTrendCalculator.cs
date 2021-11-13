using Stocks.General.Models;
using StocksProccesing.Relational.Model;
using System.Collections.Generic;

namespace StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator
{
    public interface IStocksTrendCalculator
    {
        decimal CalculateTrendFromList(List<StocksPriceData> lastRangePricesData);
        decimal CalculateTrendFromOHLC(OHLCPriceValue stocksOHLC);
        decimal CalculateTrendFromOHLC(StocksOHLC stocksOHLC);

    }
}