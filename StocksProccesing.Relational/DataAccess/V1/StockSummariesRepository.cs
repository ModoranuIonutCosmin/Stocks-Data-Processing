using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksProccesing.Relational.Model;

namespace StocksProccesing.Relational.DataAccess.V1;

public class StockSummariesRepository : Repository<StocksOhlc, int>, IStockSummariesRepository
{
    public StockSummariesRepository(StocksMarketContext context) : base(context)
    {
    }

    public StocksOhlc GetLastSummaryEntry(string ticker, TimeSpan interval)
    {
        if (string.IsNullOrWhiteSpace(ticker))
            throw new ArgumentException($"'{nameof(ticker)}' cannot be null or whitespace.", nameof(ticker));

        return _dbContext.Summaries
            .OrderBy(e => e.Date)
            .Where(e => e.CompanyTicker == ticker && e.Period == interval.Ticks)
            .AsNoTracking()
            .LastOrDefault();
    }

    public async Task<StocksOhlc> GetLastSummaryEntryForTickerAndInterval(string ticker,
        TimeSpan interval)
    {
        return await _dbContext.Summaries
            .Where(e => e.Period == interval.Ticks && e.CompanyTicker == ticker)
            .OrderByDescending(e => e.Date)
            .AsNoTracking()
            .FirstAsync();
    }

    public async Task<List<StocksOhlc>> GetLastSummaryEntryForAll(TimeSpan interval)
    {
        var dailySummaries = await _dbContext.Summaries
            .Where(e => e.Period == interval.Ticks)
            .ToListAsync();

        return dailySummaries
            .GroupBy(e => e.CompanyTicker)
            .Select(group => group
                .OrderByDescending(summary => summary.Date)
                .First()
            ).ToList();
    }

    public async Task<List<StocksOhlc>> GetAllByTickerAndPeriod(string ticker, TimeSpan period)
    {
        return (await GetAllWhereAsync(e => e.CompanyTicker == ticker &&
                                            e.Period == period.Ticks)).ToList();
    }
}