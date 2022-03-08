using System;
using System.Collections.Generic;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories.Base;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Repositories
{
    public interface IStockSummariesRepository : IRepository<StocksOhlc, int>
    {
        StocksOhlc GetLastSummaryEntry(string ticker, TimeSpan interval);
        List<StocksOhlc> GetLastSummaryEntryForAll(TimeSpan interval);
    }
}
