using System;
using Stocks.General.ExtensionMethods;

namespace StockBulkGatherer;

public class AlphaVantageContext
{
    public class ApiLimits
    {
        public int CallsPerMinute = 5;
        public int EntriesPerCall = 50000;
    }

    public ApiLimits Limits = new ApiLimits();
    public string ApiKey { get; set; } = "ItDQkGgz7847ipgJ_e11TpgPrSBDkVJr";
    public string ApiHost (string ticker, DateTimeOffset fromDate, DateTimeOffset toDate, int limit = 50000) 
        => $"https://api.polygon.io/v2/aggs/ticker/{ticker}/range/1/minute/" +
    $"{fromDate.ToUSDashDateFormat()}/{toDate.ToUSDashDateFormat()}" +
    $"?adjusted=true&sort=asc&limit={limit}&apiKey={ApiKey}";
}
