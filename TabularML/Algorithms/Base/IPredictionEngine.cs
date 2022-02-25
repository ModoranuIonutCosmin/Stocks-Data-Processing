namespace TabularML;

public interface IPredictionEngine
{
    Task SetupPipeline();
    Task TrainModel(int horizon, double testFraction);
    Task<AccuracyStatistics> EvaluateModel(int horizon, double testFraction);
    Task<List<PredictionResult>> ComputePredictionsForNextPeriod(int horizon, double testFraction);
}