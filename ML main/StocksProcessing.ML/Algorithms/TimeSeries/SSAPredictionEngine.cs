using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using StocksProcessing.ML.Algorithms.Base;
using StocksProcessing.ML.Models.TimeSeries;

namespace StocksProcessing.ML.Algorithms.TimeSeries;

public class SSAPredictionEngine : TimeSeriesPredictionEngine
{
    public SSAPredictionEngine(IEnumerable<TimestampPriceInputModel> dataset) : base(dataset)
    {
    }

    public override async Task SetupPipeline(int horizon)
    {
        var trainDataLength = _dataset.Count();

        var forecastingPipeline = MlContext.Forecasting.ForecastBySsa(
            "ForecastedPrices",
            "Price",
            horizon,
            trainDataLength,
            trainDataLength,
            horizon,
            confidenceLevel: 0.99f,
            confidenceLowerBoundColumn: "LowerBoundPrices",
            confidenceUpperBoundColumn: "UpperBoundPrices");

        TrainPipeline = forecastingPipeline;
    }
}