using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stocks.General.Models;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;
using StocksProcessing.API.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StocksProcessing.API.Controllers.v1;

[ApiVersion("1.0")]
[ApiController]
public class StocksInfoController : BaseController
{
    private readonly ICompanyService _companiesService;
    private readonly IPredictionsDataService _predictionsDataService;
    private readonly IStocksService _stocksService;

    public StocksInfoController(
        IPredictionsDataService predictionsDataService,
        IStocksService stocksService,
        ICompanyService companiesService
    )
    {
        _predictionsDataService = predictionsDataService;
        _stocksService = stocksService;
        _companiesService = companiesService;
    }


    [HttpGet("company/{ticker}")]
    public async Task<StockReportSingle> GetCompanyData(string ticker)
    {
        return await _stocksService.GetReportForSingleCompany(ticker);
    }

    [HttpGet("report")]
    public async Task<List<StockReportSingle>> GetReportsAllCompanies()
    {
        return await _stocksService.GetReportForOfAllCompanies();
    }


    [HttpGet("historicalData")]
    public async Task<StocksSummary> GetHistoricalData([NotNull] string ticker,
        [NotNull] string interval)
    {
        return await _stocksService.GetHistoricalData(ticker, interval);
    }

    [HttpGet("forecastData")]
    public async Task<StocksPredictionsPaginatedDTO> GetPredictions
    ([NotNull] string ticker,
        string algorithm = "T_FTO",
        int page = 0, int count = 1000)
    {
        return await _predictionsDataService.GatherPredictions(algorithm,
            ticker, page, count);
    }
}