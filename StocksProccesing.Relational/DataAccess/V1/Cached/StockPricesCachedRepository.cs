using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksProccesing.Relational.Cache;
using StocksProccesing.Relational.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksProccesing.Relational.DataAccess.V1.Cached
{

    public class StockPricesCachedRepository : Repository<StocksPriceData, int>, IStockPricesRepository
    {
        private readonly IStockPricesRepository _stockPricesRepository;
        private readonly ICacheService _cacheService;

        public StockPricesCachedRepository(StocksMarketContext context,
            IStockPricesRepository stockPricesRepository,
            ICacheService cacheService) : base(context)
        {
            _stockPricesRepository = stockPricesRepository;
            this._cacheService = cacheService;
        }

        public async Task<List<StocksPriceData>> GetPredictionsForTickerAndAlgorithmPaginated(
            string ticker, string algorithm, int page = 0, int count = 1000)
        {
            string cacheKey = $"predictions_by_ticker_algorithm_period({ticker},{algorithm},{page},{count})";

            List<StocksPriceData> predictions
                = await _cacheService.GetOrDefault<List<StocksPriceData>>(cacheKey);

            if (predictions == null)
            {
                predictions = await _stockPricesRepository
                    .GetPredictionsForTickerAndAlgorithmPaginated(ticker, algorithm, page, count);

                await _cacheService.Set(cacheKey, predictions, TimeSpan.FromMinutes(30));
            }

            return predictions;
        }

        public List<StocksPriceData> GetTodaysPriceEvolution(string ticker)
        {
            return _stockPricesRepository.GetTodaysPriceEvolution(ticker);
        }

        public void RemoveAllPricePredictionsForTicker(string ticker)
        {
            _stockPricesRepository.RemoveAllPricePredictionsForTicker(ticker);
        }

        public async Task RemoveAllPricePredictionsForTickerAndAlgorithm(string ticker, string algorithm)
        {
            await _stockPricesRepository.RemoveAllPricePredictionsForTickerAndAlgorithm(ticker, algorithm);
        }

        public decimal GetCurrentUnitPriceByStocksCompanyTicker(string ticker)
        {
            return _stockPricesRepository.GetCurrentUnitPriceByStocksCompanyTicker(ticker);
        }

        public Task<int> GetPredictionsCountForTickerAndAlgorithm(string ticker, string algorithm)
        {
            return _stockPricesRepository.GetPredictionsCountForTickerAndAlgorithm(ticker, algorithm);
        }
    }
}
