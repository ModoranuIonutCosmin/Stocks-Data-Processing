﻿using System;
using System.Collections.Generic;
using System.Linq;
using Stocks.General.ExtensionMethods;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksProccesing.Relational.Model;

namespace StocksProccesing.Relational.DataAccess.V1
{
    public class StockSummariesRepository : Repository<StocksOhlc, int>, IStockSummariesRepository
    {
        public StockSummariesRepository(StocksMarketContext context) : base(context)
        {
        }

        public StocksOhlc GetLastSummaryEntry(string ticker, TimeSpan interval)
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

        public List<StocksOhlc> GetLastSummaryEntryForAll(TimeSpan interval)
        {
            return TickersHelpers.GatherAllTickers()
                .Select(t => GetLastSummaryEntry(t, interval)).ToList();
        }
    }
}