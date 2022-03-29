using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Stocks_Data_Processing.Actions;
using Stocks_Data_Processing.Interfaces.Jobs;
using Stocks_Data_Processing.Interfaces.Services;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksProccesing.Relational.Model;

namespace Stocks_Data_Processing.Jobs;

/// <summary>
///     Serviciu ce updateaza valorile stock-urilor urmarite in baza de date.
/// </summary>
/// <returns></returns>
public class MaintainCurrentStockData : IMaintainCurrentStockData
{
    private readonly ICompaniesRepository _companiesRepository;

    public MaintainCurrentStockData(
        ICompaniesRepository companiesRepository,
        IStockPricesRepository stockPricesRepository,
        ICurrentStockInfoDataScraperService currentStockInfoDataScraper,
        IMaintainanceJobsRepository jobsRepository,
        ILogger<MaintainCurrentStockData> logger)
    {
        _companiesRepository = companiesRepository;
        _stockPricesRepository = stockPricesRepository;
        _currentStockInfoDataScraper = currentStockInfoDataScraper;
        _jobsRepository = jobsRepository;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await UpdateStockDataAsync();
    }

    /// <summary>
    ///     Updateaza valorile stock-urilor urmarite in baza de date.
    /// </summary>
    public async Task UpdateStockDataAsync()
    {
        _logger.LogWarning(
            $"[Current prices maintain task] Starting to update current stock data {DateTimeOffset.UtcNow}");

        _companiesRepository.EnsureCompaniesDataExists();

        //Obtine date despre stock-urile companiilor urmarite.
        var stockData = await _currentStockInfoDataScraper.GatherAllAsync();

        //Formeaza lista de randuri ce trebuie adaugate in tabelul cu preturi.
        var stocksTableEntries = stockData.Select(response =>
            new StocksPriceData
            {
                Price = response.Current,
                Prediction = false,
                Date = response.DateTime,
                CompanyTicker = response.Ticker.ToString()
            }).ToList();

        await _stockPricesRepository.AddRangeAsync(stocksTableEntries);

        await _stockPricesRepository.DeleteWhereAsync(p => p.Date < DateTimeOffset.UtcNow && p.Prediction);

        _jobsRepository.MarkJobFinished(MaintainanceTasksSchedulerHelpers.CurrentStocksJob);

        _logger.LogWarning($"[Current prices maintain task] Done to update current stock data {DateTimeOffset.UtcNow}");
    }

    #region Private members

    private readonly IStockPricesRepository _stockPricesRepository;
    private readonly ICurrentStockInfoDataScraperService _currentStockInfoDataScraper;
    private readonly IMaintainanceJobsRepository _jobsRepository;
    private readonly ILogger<MaintainCurrentStockData> _logger;

    #endregion
}