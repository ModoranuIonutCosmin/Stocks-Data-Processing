using Microsoft.ML;
using Microsoft.ML.Trainers.FastTree;

namespace TabularML;

public class TabularLbfgsPoissonRegressionPredictionEngine : TabularPredictionEngine
{
    public TabularLbfgsPoissonRegressionPredictionEngine(IEnumerable<TabularModelInput> dataset) : 
        base(dataset)
    {
    }

    public override async Task SetupPipeline()
    {
        // Data process configuration with pipeline data transformations
        
        var pipeline = MlContext.Regression.Trainers
                .LbfgsPoissonRegression(labelColumnName: @"Label", featureColumnName: @"Features",
                    historySize: 1440, enforceNonNegativity: true);
        this.TrainPipeline = pipeline;
    }
}