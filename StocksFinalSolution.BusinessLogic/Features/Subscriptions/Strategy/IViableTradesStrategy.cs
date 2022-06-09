using StocksProccesing.Relational.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StocksFinalSolution.BusinessLogic.Features.Subscriptions.Strategy
{
    public interface IViableTradesStrategy
    {
        Task<List<StocksPriceData>> ExecuteStrategy(List<StocksPriceData> nextHorizonPrices);
    }
}