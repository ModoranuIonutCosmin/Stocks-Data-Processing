using Microsoft.ML;
using StocksProcessing.ML;

namespace TabularML;

public abstract class PredictionEngineBase<TMI> : IPredictionEngine
    where TMI : class, IInputModel, new()
{
    protected IEnumerable<TMI> _dataset;
    protected IDataView TrainData { get; set; }
    protected IDataView TestData { get; set; }
    protected MLContext MlContext { get; set; }
    protected dynamic TrainPipeline { get; set; }
    
    public PredictionEngineBase(IEnumerable<TMI> dataset)
    {
        _dataset = dataset;
        MlContext = new MLContext();
    }

    public abstract Task SetupPipeline();
    public abstract Task TrainModel(int horizon, double testFraction);
    public abstract Task<List<PredictionResult>> ComputePredictionsForNextPeriod(int horizon,
        double testFraction);
    public virtual async Task<AccuracyStatistics> EvaluateModel(int horizon, double testFraction)
    {
        var predictions = Enumerable.Select<PredictionResult, float>((await ComputePredictionsForNextPeriod(horizon, testFraction)), pred => (float) pred.Price).ToList();
        var testData = MlContext.Data
            .CreateEnumerable<TMI>(TestData, reuseRowObject: true)
            .Select(data => data.GetLabel())
            .ToList();

        return new AccuracyStatistics()
        {
            MAE = predictions.CalculateMAE(testData),
            RMSE = predictions.CalculateRMSE(testData),
        };
    }
}