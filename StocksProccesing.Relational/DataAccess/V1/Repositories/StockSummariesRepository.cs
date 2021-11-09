using StocksProccesing.Relational.Model;
using System;
using System.Collections.Generic;
using Stocks.General.ExtensionMethods;
using System.Linq;

namespace StocksProccesing.Relational.DataAccess.V1.Repositories
{
    public class StockSummariesRepository : Repository<StocksOHLC, int>, IStockSummariesRepository
    {
        public StockSummariesRepository(StocksMarketContext context) : base(context)
        {
        }

        public StocksOHLC GetLastSummaryEntry(string ticker, TimeSpan interval)
        {
            if (string.IsNullOrWhiteSpace(ticker))
            {
                throw new ArgumentException($"'{nameof(ticker)}' cannot be null or whitespace.", nameof(ticker));
            }

            var result = _dbContext.Summaries
                .OrderBy(e => e.Date)
                .LastOrDefault(e => e.CompanyTicker == ticker && e.Period == interval.Ticks);

            return result;
        }

        public List<StocksOHLC> GetLastSummaryEntryForAll(TimeSpan interval)
        {
            return TickersHelpers.GatherAllTickers()
                .Select(t => GetLastSummaryEntry(t, interval)).ToList();
        }
    }
}
