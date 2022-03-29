using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ML;
using StocksProcessing.ML.Algorithms.Base;
using StocksProcessing.ML.Models.Tabular;

namespace StocksProcessing.ML.Algorithms.TabularReduction;

public class TabularFastTreeRegressionPredictionEngine : TabularPredictionEngine
{
    public TabularFastTreeRegressionPredictionEngine(IEnumerable<TabularModelInput> dataset) : 
        base(dataset)
    {
    }

    public override async Task SetupPipeline(int horizon)
    {
        string featuresColumnName = "Features";
        // Data process configuration with pipeline data transformations
        
        var pipeline = MlContext.Regression.Trainers
            .FastTree(labelColumnName: @"Label", featureColumnName: @"Features", learningRate:0.3f,
                numberOfLeaves: 20, numberOfTrees: 100);
        this.TrainPipeline = pipeline;
    }
}