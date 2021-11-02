using Microsoft.EntityFrameworkCore;
using Stocks.General.ExtensionMethods;
using StocksProccesing.Relational.DataAccess;
using System;
using System.Linq;

namespace StocksProccesing.Relational.Helpers
{
    public static class StocksDataAcessHelpers
    {
        public static decimal GatherCurrentPriceByCompany(this StocksMarketContext context, string ticker)
        {
            //TODO: MODIFY THIS
            var latestTradingDayStart = DateTimeOffset.UtcNow.AddDays(-10)
                .GetClosestPreviousStockMarketDateTime();

            var results = context.PricesData
                .Where(e => e.Date >= latestTradingDayStart && e.CompanyTicker == ticker)
                .AsNoTracking()
                .ToList();

            if(results.Count == 0)
            {
                throw new Exception("No available last price!");
            }

            return results.Last().Price;
        }
    }
}
