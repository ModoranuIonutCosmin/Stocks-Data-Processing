using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers.FastTree;
using StocksProcessing.ML;
using StocksProcessing.ML.Models;

namespace TabularML;

public class TabularFastTreeRegressionPredictionEngine : TabularPredictionEngine
{
    public TabularFastTreeRegressionPredictionEngine(IEnumerable<TabularModelInput> dataset) : 
        base(dataset)
    {
    }

    public override async Task SetupPipeline()
    {
        string featuresColumnName = "Features";
        // Data process configuration with pipeline data transformations
        
        var pipeline = MlContext.Regression.Trainers
            .FastTree(labelColumnName: @"Label", featureColumnName: @"Features", learningRate:0.3f,
                numberOfLeaves: 20, numberOfTrees:100);
        this.TrainPipeline = pipeline;
    }
}