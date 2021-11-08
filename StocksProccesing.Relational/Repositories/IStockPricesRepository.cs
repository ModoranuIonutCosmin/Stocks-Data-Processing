using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StocksProccesing.Relational.Repositories
{
    public interface IStockPricesRepository : IEFRepository<StocksMarketContext>
    {
        Task AddPricesDataAsync(List<StocksPriceData> elements);
        decimal GetCurrentUnitPriceByStocksCompanyTicker(string ticker);
        List<StocksPriceData> GetTodaysPriceEvolution(string ticker);
        void RemoveAllPricePredictionsForTicker(string ticker);
    }
}