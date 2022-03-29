using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using StocksProcessing.ML.Models.TimeSeries;

namespace StocksProcessing.ML.Algorithms.Base;

public abstract class
    TimeSeriesPredictionEngine : PredictionEngineBase<TimestampPriceInputModel, TimestampPriceOutputModel>
{
    public TimeSeriesPredictionEngine(IEnumerable<TimestampPriceInputModel> dataset) : base(dataset)
    {
    }

    public override SchemaDefinition CreateCustomSchemaDefinition()
    {
        return null;
    }

    public override async Task CreatePredictionEngine(ITransformer model)
    {
        PredictionEngine = model.CreateTimeSeriesEngine<TimestampPriceInputModel,
            TimestampPriceOutputModel>(MlContext);
    }

    public override async Task<List<PredictionResult>> ComputePredictionsForNextPeriod
        (int horizon, double testFraction)
    {
        if (PredictionEngine == null) await TrainModel(horizon, testFraction);

        var predictionEngine =
            PredictionEngine as TimeSeriesPredictionEngine<TimestampPriceInputModel, TimestampPriceOutputModel>;

        var results = predictionEngine.Predict();

        return results.ForecastedPrices.Select(price => new PredictionResult
        {
            Price = price
        }).ToList();
    }
}