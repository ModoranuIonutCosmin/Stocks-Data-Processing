using StocksProccesing.Relational.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StocksFinalSolution.BusinessLogic.Features.Subscriptions.Strategy;

public class ShortTermBUYPeaksAndValleysStrategy : IViableTradesStrategy
{
    public Task<List<StocksPriceData>> ExecuteStrategy(List<StocksPriceData> nextHorizonPrices)
    {
        var result = new List<StocksPriceData>();
        int currentPosition = 0;

        if (nextHorizonPrices == null || !nextHorizonPrices.Any())
        {
            return Task.FromResult(new List<StocksPriceData>());
        }

        int observationsCount = nextHorizonPrices.Count;

        nextHorizonPrices = nextHorizonPrices.OrderBy(observation => observation.Date)
                                             .ToList();

        int valleyIndex = 0;
        int peakIndex = 0;

        while (currentPosition < observationsCount - 1)
        {
            while (currentPosition < observationsCount - 1 &&
                nextHorizonPrices[currentPosition].Price >= nextHorizonPrices[currentPosition + 1].Price)
            {
                currentPosition++;
            }

            valleyIndex = currentPosition;

            while (currentPosition < observationsCount - 1 &&
                nextHorizonPrices[currentPosition].Price <= nextHorizonPrices[currentPosition + 1].Price)
            {
                currentPosition++;
            }

            peakIndex = currentPosition;

            result.Add(nextHorizonPrices[valleyIndex]);
            result.Add(nextHorizonPrices[peakIndex]);
        }

        return Task.FromResult(result);
    }
}
