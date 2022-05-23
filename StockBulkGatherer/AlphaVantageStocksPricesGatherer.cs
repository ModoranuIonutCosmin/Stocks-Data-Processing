using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StocksProccesing.Relational.Model;

namespace StockBulkGatherer;

public class AlphaVantageStocksPricesGatherer : IApiStockPricesGatherer
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AlphaVantageStocksPricesGatherer> _logger;
    private static int RETRY_COUNT = 3;

    public AlphaVantageStocksPricesGatherer(HttpClient httpClient, 
        ILogger<AlphaVantageStocksPricesGatherer> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    public async Task<(List<StocksPriceData> results, int callsMade)> Gather(string ticker, DateTimeOffset from, DateTimeOffset to,
        int entriesLimit = 100000)
    {
        var callsNumber = 0;
        var failCount = 0;
        var pricesData = new List<StocksPriceData>();
        var alphaVantageContext = new AlphaVantageContext();

        var step = TimeSpan.FromDays(30);
        pricesData.Capacity = entriesLimit + 1;
        
        _logger.LogWarning($"Started to gather prices for {ticker}");
        
        while (from < to)
        {
            callsNumber++;
            
            if (callsNumber % alphaVantageContext.Limits.CallsPerMinute == 0)
            {
                await Task.Delay(TimeSpan.FromMinutes(1));
            }

            HttpResponseMessage response = null;
            while (RETRY_COUNT > failCount)
            {
                try
                {
                    var requestLink = alphaVantageContext.ApiHost(ticker, from, from.Add(step));
                    
                    response = await _httpClient
                        .GetAsync(requestLink);

                    response.EnsureSuccessStatusCode();

                    failCount = 0;
                    
                    break;
                }
                catch
                {
                    failCount++;
                    await Task.Delay(TimeSpan.FromSeconds(30));
                    _logger.LogWarning($"Failure count #{failCount}. Retrying...");
                }
            }

            if (RETRY_COUNT == failCount || response is null)
            {
                _logger.LogCritical($"Failed to gather data for {ticker} after {RETRY_COUNT} tries.");
                return (new List<StocksPriceData>(), callsNumber);
            }
            
            var serializedPrices = await response.Content.ReadAsStringAsync();
            var deserializedPrices = serializedPrices.DeserializeStockDataForTicker(ticker);
            
            pricesData.AddRange(deserializedPrices);

            from = from.Add(step);
            from = from.Add(TimeSpan.FromMinutes(1));
        }
        
        _logger.LogWarning($"Brought back {pricesData.Count} entries for {ticker}");

        return (pricesData, callsNumber);
    }
}