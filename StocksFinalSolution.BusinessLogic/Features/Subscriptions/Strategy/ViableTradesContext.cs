using StocksProccesing.Relational.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StocksFinalSolution.BusinessLogic.Features.Subscriptions.Strategy
{
    public class ViableTradesContext
    {
        IViableTradesStrategy _viableTradesStrategy;

        public ViableTradesContext()
        {
            _viableTradesStrategy = new ShortTermBUYPeaksAndValleysStrategy();
        }

        public void SetStrategy (ViableTradesStrategy tradesStrategy)
        {
            switch (tradesStrategy)
            {
                case ViableTradesStrategy.ShortTermBUY:
                    _viableTradesStrategy = new ShortTermBUYPeaksAndValleysStrategy();
                    break;

                case ViableTradesStrategy.ShortTermSELL:
                    _viableTradesStrategy = new ShortTermSellPeaksAndValleysStrategy();
                    break;
            }
        }

        public async Task<List<StocksPriceData>> Execute(List<StocksPriceData> nextHorizonObservations)
        {
            return await _viableTradesStrategy.ExecuteStrategy(nextHorizonObservations);
        }
    }
}
