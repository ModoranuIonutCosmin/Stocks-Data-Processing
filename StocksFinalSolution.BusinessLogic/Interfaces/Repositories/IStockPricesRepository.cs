using System.Collections.Generic;
using System.Threading.Tasks;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories.Base;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Repositories;

public interface IStockPricesRepository : IRepository<StocksPriceData, int>
{
    decimal GetCurrentUnitPriceByStocksCompanyTicker(string ticker);
    List<StocksPriceData> GetTodaysPriceEvolution(string ticker);

    Task<List<StocksPriceData>> GetPredictionsForTickerAndAlgorithmPaginated(string ticker, string algorithm,
        int page, int count);

    Task<int> GetPredictionsCountForTickerAndAlgorithm(string ticker, string algorithm);
    void RemoveAllPricePredictionsForTicker(string ticker);
    Task RemoveAllPricePredictionsForTickerAndAlgorithm(string ticker, string algorithm);
}