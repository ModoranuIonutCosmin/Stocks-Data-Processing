using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using StocksProcessing.ML.Models;

namespace StocksProcessing.ML.Algorithms.Base;

public interface IPredictionEngine
{
    Task SetupPipeline(int horizon);
    Task CreatePredictionEngine(ITransformer model);
    Task TrainModel(int horizon, double testFraction);
    SchemaDefinition CreateCustomSchemaDefinition();

    Task<(AccuracyStatistics accuracy, List<PredictionResult> predictions)> EvaluateModel(int horizon,
        double testFraction, TimeSpan interval);

    Task<List<PredictionResult>> ComputePredictionsForNextPeriod(int horizon, double testFraction);
}