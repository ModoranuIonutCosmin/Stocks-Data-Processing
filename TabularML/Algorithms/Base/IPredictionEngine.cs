using Microsoft.ML;

namespace TabularML.Algorithms.Base;

public interface IPredictionEngine
{
    Task SetupPipeline(int horizon);
    Task CreatePredictionEngine(ITransformer model);
    Task TrainModel(int horizon, double testFraction);
    Task<AccuracyStatistics> EvaluateModel(int horizon, double testFraction);
    Task<List<PredictionResult>> ComputePredictionsForNextPeriod(int horizon, double testFraction);
}