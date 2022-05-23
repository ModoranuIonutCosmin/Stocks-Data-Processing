using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StocksProccesing.Relational.Model;

namespace StockBulkGatherer;

public interface IApiStockPricesGatherer
{
    Task<(List<StocksPriceData> results, int callsMade)> Gather(string ticker, DateTimeOffset from, DateTimeOffset to,
        int entriesLimit = 100000);
}