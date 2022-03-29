using System;
using System.Threading.Tasks;
using StocksProcessing.API.Models;

namespace StocksFinalSolution.BusinessLogic.Features.StocksMarketSummaryGenerator;

public interface IStocksSummaryGenerator
{
    Task<StocksSummary> GenerateSummary(string ticker, TimeSpan interval);
}