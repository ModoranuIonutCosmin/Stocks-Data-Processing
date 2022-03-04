using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ML;
using StocksProcessing.ML.Models;

namespace StocksProcessing.ML.Algorithms.Base;

public interface IPredictionEngine
{
    Task SetupPipeline(int horizon);
    Task CreatePredictionEngine(ITransformer model);
    Task TrainModel(int horizon, double testFraction);
    Task<AccuracyStatistics> EvaluateModel(int horizon, double testFraction);
    Task<List<PredictionResult>> ComputePredictionsForNextPeriod(int horizon, double testFraction);
}