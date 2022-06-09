using Stocks.General.Entities;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksProccesing.Relational.Cache;
using StocksProccesing.Relational.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StocksProccesing.Relational.DataAccess.V1.Cached
{
    public class StocksSummariesCachedRepository: Repository<StocksOhlc, int>, IStockSummariesRepository
    {
        private readonly IStockSummariesRepository _stockSummariesRepository;
        private readonly ICacheService _cacheService;

        public StocksSummariesCachedRepository(IStockSummariesRepository stockSummariesRepository,
            ICacheService cacheService,
            StocksMarketContext context)
            : base(context)
        {
            _stockSummariesRepository = stockSummariesRepository;
            _cacheService = cacheService;
        }

        public async Task<List<StocksOhlc>> GetAllByTickerAndPeriod(string ticker, TimeSpan period)
        {
            string cacheKey = $"summary_ticker_period({ticker},{period.Ticks})";

            List<StocksOhlc> stocksByPeriod
                = await _cacheService.GetOrDefault<List<StocksOhlc>>(cacheKey);

            if (stocksByPeriod == null)
            {
                stocksByPeriod = await _stockSummariesRepository.GetAllByTickerAndPeriod(ticker, period);

                await _cacheService.Set(cacheKey, stocksByPeriod, period);
            }

            return stocksByPeriod;
        }


        public StocksOhlc GetLastSummaryEntry(string ticker, TimeSpan interval)
        {
            return _stockSummariesRepository.GetLastSummaryEntry(ticker, interval);
        }

        public async Task<List<StocksOhlc>> GetLastSummaryEntryForAll(TimeSpan interval)
        {
            return await _stockSummariesRepository.GetLastSummaryEntryForAll(interval);
        }

        public async Task<StocksOhlc> GetLastSummaryEntryForTickerAndInterval(string ticker, TimeSpan interval)
        {
            return await _stockSummariesRepository.GetLastSummaryEntryForTickerAndInterval(ticker, interval);
        }
    }
}
