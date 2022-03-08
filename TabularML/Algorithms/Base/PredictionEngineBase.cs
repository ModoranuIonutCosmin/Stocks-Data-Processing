using Microsoft.ML;
using StocksProcessing.ML;
using StocksProcessing.ML.Models;

namespace TabularML.Algorithms.Base;

public abstract class PredictionEngineBase<TMI, TMO> : IPredictionEngine
    where TMI : class, IInputModel, new()
    where TMO: class, new()
{
    protected IEnumerable<TMI> _dataset;
    protected IDataView TrainData { get; set; }
    protected IDataView TestData { get; set; }
    protected MLContext MlContext { get; set; }
    protected dynamic TrainPipeline { get; set; }
    
    protected Microsoft.ML.PredictionEngineBase<TMI, TMO> PredictionEngine;
    
    public PredictionEngineBase(IEnumerable<TMI> dataset)
    {
        _dataset = dataset;
        MlContext = new MLContext();
    }

    public abstract Task SetupPipeline(int horizon);
    public virtual async Task CreatePredictionEngine(ITransformer model)
    {
        PredictionEngine = MlContext.Model.CreatePredictionEngine<TMI, TMO>(model);
    }
    public virtual async Task TrainModel(int horizon, double testFraction)
    {
        SeparatedDataset<TMI> separatedDataset = _dataset.SeparateDataSet(testFraction);

        TrainData = MlContext.Data.LoadFromEnumerable(separatedDataset.TrainData);
        TestData = MlContext.Data.LoadFromEnumerable(separatedDataset.TestData);
        dynamic model;
        await SetupPipeline(horizon);
        try
        {
            model = TrainPipeline.Fit(TrainData);
            this.CreatePredictionEngine(model);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public abstract Task<List<PredictionResult>> ComputePredictionsForNextPeriod(int horizon,
        double testFraction);
    public virtual async Task<AccuracyStatistics> EvaluateModel(int horizon, double testFraction)
    {
        var predictions = Enumerable.Select(
                                    await ComputePredictionsForNextPeriod(horizon, testFraction),
                                    pred => (float) pred.Price).ToList();
        
        var testData = MlContext.Data
            .CreateEnumerable<TMI>(TestData, reuseRowObject: false)
            .Select(data => data.GetLabel())
            .ToList();

        return new AccuracyStatistics()
        {
            MAE = predictions.CalculateMAE(testData),
            RMSE = predictions.CalculateRMSE(testData),
        };
    }
}