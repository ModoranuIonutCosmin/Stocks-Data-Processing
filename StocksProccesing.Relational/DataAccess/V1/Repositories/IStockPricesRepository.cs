using StocksProccesing.Relational.Interfaces;
using StocksProccesing.Relational.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StocksProccesing.Relational.Repositories
{
    public interface IStockPricesRepository : IRepository<StocksPriceData, int>
    {
        Task AddPricesDataAsync(List<StocksPriceData> elements);
        decimal GetCurrentUnitPriceByStocksCompanyTicker(string ticker);
        List<StocksPriceData> GetTodaysPriceEvolution(string ticker);
        void RemoveAllPricePredictionsForTicker(string ticker);
    }
}