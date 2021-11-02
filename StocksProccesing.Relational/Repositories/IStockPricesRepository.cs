using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Model;
using System.Collections.Generic;

namespace StocksProccesing.Relational.Repositories
{
    public interface IStockPricesRepository : IEFRepository<StocksMarketContext>
    {
        decimal GetCurrentUnitPriceByStocksCompanyTicker(string ticker);
        List<StocksPriceData> GetTodaysPriceEvolution(string ticker);
    }
}