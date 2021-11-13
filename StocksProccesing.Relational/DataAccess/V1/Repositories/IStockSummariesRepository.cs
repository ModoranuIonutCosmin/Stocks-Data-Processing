using StocksProccesing.Relational.Interfaces;
using StocksProccesing.Relational.Model;
using System;
using System.Collections.Generic;

namespace StocksProccesing.Relational.DataAccess.V1.Repositories
{
    public interface IStockSummariesRepository : IRepository<StocksOhlc, int>
    {
        StocksOhlc GetLastSummaryEntry(string ticker, TimeSpan interval);
        List<StocksOhlc> GetLastSummaryEntryForAll(TimeSpan interval);
    }
}
