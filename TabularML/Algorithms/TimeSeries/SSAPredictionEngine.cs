using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using StocksProcessing.ML.Models;
using TabularML.Algorithms.Base;

namespace TabularML.Algorithms.TimeSeries;

public class SSAPredictionEngine: TimeSeriesPredictionEngine
{
    public SSAPredictionEngine(IEnumerable<TimestampPriceInputModel> dataset) : base(dataset)
    {
    }

    public override async Task SetupPipeline(int horizon)
    {
        var trainDataLength = _dataset.Count();

        var forecastingPipeline = MlContext.Forecasting.ForecastBySsa(
            outputColumnName: "ForecastedPrices",
            inputColumnName: "Price",
            windowSize: horizon,
            seriesLength: trainDataLength,
            trainSize: trainDataLength,
            horizon: horizon,
            confidenceLevel: 0.99f,
            confidenceLowerBoundColumn: "LowerBoundPrices",
            confidenceUpperBoundColumn: "UpperBoundPrices");

        this.TrainPipeline = forecastingPipeline;
    }
}