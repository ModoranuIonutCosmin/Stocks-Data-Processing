using System.Collections.Generic;
using Stocks.General.Models;
using Stocks.General.Models.StocksInfoAggregates;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Services
{
    public interface IStocksTrendCalculator
    {
        decimal CalculateTrendFromList(List<StocksPriceData> lastRangePricesData);
        decimal CalculateTrendFromOHLC(OhlcPriceValue stocksOHLC);
        decimal CalculateTrendFromOHLC(StocksOhlc stocksOHLC);

    }
}