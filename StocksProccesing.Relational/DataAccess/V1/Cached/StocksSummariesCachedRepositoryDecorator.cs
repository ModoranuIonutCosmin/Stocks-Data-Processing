using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksProccesing.Relational.Cache;
using StocksProccesing.Relational.DataAccess.V1.Decorator;
using StocksProccesing.Relational.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StocksProccesing.Relational.DataAccess.V1.Cached
{
    public class StocksSummariesCachedRepository: StockSummariesRepositoryDecorator
    {
        private readonly IStockSummariesRepository _stockSummariesRepository;
        private readonly ICacheService _cacheService;

        public StocksSummariesCachedRepository(IStockSummariesRepository stockSummariesRepository,
            ICacheService cacheService,
            StocksMarketContext context)
            : base(context, stockSummariesRepository)
        {
            _stockSummariesRepository = stockSummariesRepository;
            _cacheService = cacheService;
        }

        public override async Task<List<StocksOhlc>> GetAllEntriesByTickerAndPeriod(string ticker, TimeSpan period)
        {
            string cacheKey = $"summary_ticker_period({ticker},{period.Ticks})";

            List<StocksOhlc> stocksByPeriod
                = await _cacheService.GetOrDefault<List<StocksOhlc>>(cacheKey);

            if (stocksByPeriod == null)
            {
                stocksByPeriod = await _stockSummariesRepository.GetAllEntriesByTickerAndPeriod(ticker, period);

                await _cacheService.Set(cacheKey, stocksByPeriod, period);
            }

            return stocksByPeriod;
        }
    }
}
