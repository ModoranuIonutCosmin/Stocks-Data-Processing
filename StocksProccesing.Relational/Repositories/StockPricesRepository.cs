using Microsoft.EntityFrameworkCore;
using Stocks.General.ExtensionMethods;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StocksProccesing.Relational.Repositories
{
    public class StockPricesRepository : IStockPricesRepository
    {
        public StocksMarketContext _dbContext { get; set; }

        public StockPricesRepository(StocksMarketContext dbContext)
        {
            _dbContext = dbContext;
        }

        public decimal GetCurrentUnitPriceByStocksCompanyTicker(string ticker)
            => _dbContext.PricesData.OrderBy(e => e.Date)
            .LastOrDefault(e => e.CompanyTicker == ticker).Price;

        public List<StocksPriceData> GetTodaysPriceEvolution(string ticker)
        {
            var currentDateUTC = DateTimeOffset.UtcNow.AddDays(-20).SetTime(8, 0);
            var todaysPrices = new List<StocksPriceData>();

                return todaysPrices = _dbContext.PricesData
                                .Where(e => e.Date >= currentDateUTC && e.CompanyTicker == ticker.ToUpper())
                                .OrderBy(e => e.Date)
                                .AsNoTracking()
                                .ToList();
        }
    }
}
