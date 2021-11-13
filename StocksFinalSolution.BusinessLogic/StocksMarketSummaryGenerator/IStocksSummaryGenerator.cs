using StocksProcessing.API.Models;
using System;
using System.Threading.Tasks;

namespace StocksFinalSolution.BusinessLogic.StocksMarketSummaryGenerator
{
    public interface IStocksSummaryGenerator
    {
        Task<StocksSummary> GenerateSummary(string ticker, TimeSpan interval);
    }
}