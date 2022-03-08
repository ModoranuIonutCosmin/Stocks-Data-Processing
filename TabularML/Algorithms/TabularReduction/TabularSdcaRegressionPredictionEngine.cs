using Microsoft.ML;
using TabularML.Algorithms.Base;

namespace TabularML.Algorithms.TabularReduction;

public class TabularSdcaRegressionPredictionEngine : TabularPredictionEngine
{
    public TabularSdcaRegressionPredictionEngine(IEnumerable<TabularModelInput> dataset) : 
        base(dataset)
    {
    }

    public override async Task SetupPipeline(int horizon)
    {
        var pipeline = MlContext.Regression.Trainers
                .Sdca(labelColumnName: @"Label", featureColumnName: @"Features");
        this.TrainPipeline = pipeline;
    }
}