using Microsoft.EntityFrameworkCore;
using Stocks.General.ExtensionMethods;
using StocksProccesing.Relational.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StocksProccesing.Relational.DataAccess.V1.Repositories
{
    public class StockPricesRepository : Repository<StocksPriceData, int>,
        IStockPricesRepository
    {
        public StockPricesRepository(StocksMarketContext context) : base(context)
        {
        }

        public decimal GetCurrentUnitPriceByStocksCompanyTicker(string ticker)
            => _dbContext.PricesData.OrderBy(e => e.Date)
            .LastOrDefault(e => e.CompanyTicker == ticker).Price;

        public List<StocksPriceData> GetTodaysPriceEvolution(string ticker)
        {
            var currentDateUTC = DateTimeOffset.UtcNow.SetTime(8, 0);

            return _dbContext.PricesData
                            .Where(e => e.Date >= currentDateUTC && e.CompanyTicker == ticker.ToUpper())
                            .OrderBy(e => e.Date)
                            .AsNoTracking()
                            .ToList();
        }

        public async Task AddPricesDataAsync(List<StocksPriceData> elements)
        {
            await _dbContext.PricesData.AddRangeAsync(elements);
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
