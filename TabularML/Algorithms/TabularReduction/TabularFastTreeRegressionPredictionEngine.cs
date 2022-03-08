using Microsoft.ML;
using TabularML.Algorithms.Base;

namespace TabularML.Algorithms.TabularReduction;

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
                numberOfLeaves: 20, numberOfTrees:100);
        this.TrainPipeline = pipeline;
    }
}