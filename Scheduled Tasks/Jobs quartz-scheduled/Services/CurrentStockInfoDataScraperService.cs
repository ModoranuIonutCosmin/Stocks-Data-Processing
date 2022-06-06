using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Stocks.General;
using Stocks.General.ExtensionMethods;
using Stocks_Data_Processing.Interfaces.Services;
using Stocks_Data_Processing.Models;

namespace Stocks_Data_Processing.Services;

public class CurrentStockInfoDataScraperService : ICurrentStockInfoDataScraperService
{
    private readonly ICurrentStockInfoYahooScraperService _currentStockYahooService;
    private readonly ICurrentStockInfoGoogleScraperService _currentStockGoogleService;
    private readonly ILogger<CurrentStockInfoDataScraperService> _logger;
    public CurrentStockInfoDataScraperService(
        ICurrentStockInfoYahooScraperService currentStockYahooService,
        ICurrentStockInfoGoogleScraperService currentStockGoogleService,
        ILogger<CurrentStockInfoDataScraperService> logger)
    {
        _currentStockYahooService = currentStockYahooService;
        _currentStockGoogleService = currentStockGoogleService;
        _logger = logger;
    }
    public async Task<StockCurrentInfoResponse> GatherAsync(string ticker)
    {
        //Incearca sa faca scrape la Google Finance si apoi salveaza datele obtinute in urma
        //procesului inclusiv daca a fost cu success.
        var stockDataResponse = await _currentStockGoogleService.GatherAsync(ticker);

        if (!stockDataResponse.Successful)
            //Daca scraping-ul la Google Finance a esuat...
            //... obtine datele de pe Yahoo Finance.
            stockDataResponse = await _currentStockYahooService.GatherAsync(ticker);

        if (!stockDataResponse.Successful)
            //Daca amandoua modalitati esueaza...
            //... logheaza faptul ca a esuat.
            _logger.LogCritical($"Gathering data failed for ticker {ticker}");

        return stockDataResponse;
    }

    public async Task<IList<StockCurrentInfoResponse>> GatherAllAsync()
    {
        var gatherTasks = new List<Task<StockCurrentInfoResponse>>();
        var watchList = TickersHelpers.GatherAllTickers();

        foreach (var ticker in watchList)
            //Pentru fiecare companie pe care o urmarim...
            //... porneste procesul de obtinerea valorii stock-ului.
            gatherTasks.Add(GatherAsync(ticker));

        //Returneaza valorile pentru fiecare companie cand se termina task-urile
        //aferente obtinerii lor.
        return await Task.WhenAll(gatherTasks);
    }

}