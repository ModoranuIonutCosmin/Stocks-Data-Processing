using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stocks.General.ExtensionMethods;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksProccesing.Relational.Model;

namespace StocksProccesing.Relational.DataAccess.V1
{
    public class StockPricesRepository : Repository<StocksPriceData, int>,
        IStockPricesRepository
    {
        public StockPricesRepository(StocksMarketContext context) : base(context)
        {
        }

        public decimal GetCurrentUnitPriceByStocksCompanyTicker(string ticker)
            => _dbContext.PricesData.OrderBy(e => e.Date)
            .LastOrDefault(e => e.CompanyTicker == ticker && !e.Prediction)?.Price ?? 0;

        public List<StocksPriceData> GetTodaysPriceEvolution(string ticker)
        {
            var currentDateUTC = DateTimeOffset.UtcNow.SetTime(8, 0);

            return _dbContext.PricesData
                            .Where(e => e.Date >= currentDateUTC && e.CompanyTicker == ticker.ToUpper())
                            .OrderBy(e => e.Date)
                            .AsNoTracking()
                            .ToList();
        }

        public async Task<List<StocksPriceData>> GetPredictionsForTickerAndAlgorithmPaginated(string ticker, string algorithm, int page,
            int count)
        {
            return await _dbContext.PricesData
                .Where(p => p.CompanyTicker == ticker &&
                            p.AlgorithmUsed == algorithm &&
                            p.Prediction == true)
                .OrderByDescending(p => p.Date)
                .Skip(page * count)
                .Take(count)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<int> GetPredictionsCountForTickerAndAlgorithm(string ticker, string algorithm)
        {
            return await _dbContext.PricesData
                .Where(p => p.CompanyTicker == ticker &&
                            p.AlgorithmUsed == algorithm &&
                            p.Prediction == true)
                .CountAsync();
        }

        public async Task AddPricesDataAsync(List<StocksPriceData> elements)
        {
            await _dbContext.PricesData.AddRangeAsync(elements);
            await SaveChangesAsync();
        }

        public async Task RemoveAllPricePredictionsForTickerAndAlgorithm(string ticker, string algorithm)
        {
            _dbContext.PricesData.RemoveRange(_dbContext.PricesData.Where(k => k.Prediction
                                                                               && k.CompanyTicker == ticker
                                                                               && k.AlgorithmUsed == algorithm));
            await SaveChangesAsync();
        }

        public async void RemoveAllPricePredictionsForTicker(string ticker)
        {
            _dbContext.PricesData.RemoveRange(_dbContext.PricesData.Where(k => k.Prediction
            && k.CompanyTicker == ticker));
            await SaveChangesAsync();
        }
    }
}
