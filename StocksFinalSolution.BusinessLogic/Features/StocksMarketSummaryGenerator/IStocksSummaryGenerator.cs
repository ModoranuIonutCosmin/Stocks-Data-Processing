using System;
using System.Threading.Tasks;
using Stocks.General.Entities;

namespace StocksFinalSolution.BusinessLogic.Features.StocksMarketSummaryGenerator;

public interface IStocksSummaryGenerator
{
    Task<StocksSummary> GenerateSummary(string ticker, TimeSpan interval);
}