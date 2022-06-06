using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stocks.General.ExtensionMethods;
using Stocks_Data_Processing.Interfaces.Services;
using Stocks_Data_Processing.Models;

namespace Stocks_Data_Processing.Services;

public class CurrentStockInfoGoogleScraperService : ICurrentStockInfoGoogleScraperService
{
    private readonly IScraperService _scraper;
    private const string GoogleLink = "https://www.google.com/finance?q=";

    private const string ValueXPath = @"//div[@jscontroller='NdbN0c' and
                                        @data-currency-code='USD' and 
                                        @data-is-crypto='false']//div[contains(text(), '$')]";

    public CurrentStockInfoGoogleScraperService(
        IScraperService scraper)
    {
        _scraper = scraper;
    }


    public async Task<StockCurrentInfoResponse> GatherAsync(string ticker)
    {
        decimal currentPrice;
        DateTimeOffset scrapeDate;

        try
        {
            currentPrice =
                (await _scraper.ExtractNumericFields(BuildResourceLink(ticker), ValueXPath))
                .Last();
            
            scrapeDate = DateTimeOffset.UtcNow.RoundDown(TimeSpan.FromMinutes(1));
        }

        catch (Exception ex)
        {
            return new StockCurrentInfoResponse()
            {
                Exception = ex,
                Ticker = ticker
            };
        }

        return new StockCurrentInfoResponse()
        {
            Current = currentPrice,
            Ticker = ticker,
            DateTime = scrapeDate,
        };
    }

    public string BuildResourceLink(string ticker)
    {
        return $"{GoogleLink}{ticker}";
    }
}