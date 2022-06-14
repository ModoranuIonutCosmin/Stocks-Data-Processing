using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories.Base;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Repositories;

public interface IStockSummariesRepository : IRepository<StocksOhlc, int>
{
    StocksOhlc GetLastSummaryEntryForTicker(string ticker, TimeSpan interval);
    Task<List<StocksOhlc>> GetLastSummaryEntryForAllTickers(TimeSpan interval);
    Task<List<StocksOhlc>> GetAllEntriesByTickerAndPeriod(string ticker, TimeSpan period);

    Task<StocksOhlc> GetLastSummaryEntryForTickerAndInterval(string ticker,
        TimeSpan interval);
}