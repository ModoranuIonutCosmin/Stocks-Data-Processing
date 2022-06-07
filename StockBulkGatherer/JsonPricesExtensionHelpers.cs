using System;
using System.Collections.Generic;
using System.Linq;
using Json.Net;
using Newtonsoft.Json;
using StocksProccesing.Relational.Model;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace StockBulkGatherer;

public static class JsonPricesExtensionHelpers
{
    public static List<StocksPriceData> DeserializeStockDataForTicker(this string jsonText, string ticker)
    {
        StockPricesAPIModel payload = JsonConvert.DeserializeObject<StockPricesAPIModel>(jsonText);

        return payload?.Results
            ?.Select(item => new StocksPriceData()
            {
                CompanyTicker = ticker,
                Date = DateTimeOffset.FromUnixTimeSeconds((long) item.t / 1000).ToUniversalTime(),
                Price = item.c,
                Prediction = false
            }).ToList() ?? new();
    }
}