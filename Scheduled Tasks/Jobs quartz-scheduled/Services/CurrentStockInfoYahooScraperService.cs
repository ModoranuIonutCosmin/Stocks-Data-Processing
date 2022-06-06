using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stocks.General.ExtensionMethods;
using Stocks_Data_Processing.Interfaces.Services;
using Stocks_Data_Processing.Models;

namespace Stocks_Data_Processing.Services;

public class CurrentStockInfoYahooScraperService : ICurrentStockInfoYahooScraperService
{
    private readonly IScraperService _scraper;

    private const string YahooFinanceLink = "https://finance.yahoo.com/quote/";
    
    private const string ValueXPath = @"//div[@id='quote-header-info']//
                                        *[contains(@data-field, 'Price')]";

    public CurrentStockInfoYahooScraperService(
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
        return $"{YahooFinanceLink}{ticker}";
    }

}