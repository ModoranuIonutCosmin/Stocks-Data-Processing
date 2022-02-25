using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers.FastTree;
using StocksProcessing.ML;
using StocksProcessing.ML.Models;

namespace TabularML;

public class TabularSdcaRegressionPredictionEngine : TabularPredictionEngine
{
    public TabularSdcaRegressionPredictionEngine(IEnumerable<TabularModelInput> dataset) : 
        base(dataset)
    {
    }

    public override async Task SetupPipeline()
    {
        var pipeline = MlContext.Regression.Trainers
                .Sdca(labelColumnName: @"Label", featureColumnName: @"Features");
        this.TrainPipeline = pipeline;
    }
}