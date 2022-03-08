using Microsoft.ML;
using StocksProcessing.ML;
using StocksProcessing.ML.Models;

namespace TabularML.Algorithms.Base;

public abstract class TabularPredictionEngine : PredictionEngineBase<TabularModelInput, TabularModelOutput>
{
    protected TabularPredictionEngine(IEnumerable<TabularModelInput> dataset) : base(dataset)
    {
    }

    public override async Task<List<PredictionResult>> ComputePredictionsForNextPeriod(int horizon,
        double testFraction)
    {
        if (PredictionEngine == null)
        {
            await TrainModel(horizon, testFraction);
        }

        if (PredictionEngine == null)
        {
            throw new Exception();
        }

        var result = new List<PredictionResult>();
        var previousDataEntry = _dataset.Last();

        previousDataEntry.Next = 0;

        for (int predictionIndex = 0; predictionIndex < horizon; predictionIndex++)
        {
            var nextPrice = this.PredictionEngine.Predict(previousDataEntry);

            float[] nextFeatures = new float[previousDataEntry.Features.Length];
            Array.Copy(previousDataEntry.Features, 1, nextFeatures,
                0, previousDataEntry.Features.Length - 1);
            nextFeatures[^1] = nextPrice.Score;
            previousDataEntry.Features = nextFeatures;

            result.Add(new PredictionResult()
            {
                Price = (decimal) nextPrice.Score
            });
        }

        return result;
    }
}