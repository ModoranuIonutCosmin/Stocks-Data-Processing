using Microsoft.ML;
using TabularML.Algorithms.Base;

namespace TabularML.Algorithms.TabularReduction;

public class TabularLbfgsPoissonRegressionPredictionEngine : TabularPredictionEngine
{
    public TabularLbfgsPoissonRegressionPredictionEngine(IEnumerable<TabularModelInput> dataset) : 
        base(dataset)
    {
    }

    public override async Task SetupPipeline(int horizon)
    {
        // Data process configuration with pipeline data transformations
        
        var pipeline = MlContext.Regression.Trainers
                .LbfgsPoissonRegression(labelColumnName: @"Label", featureColumnName: @"Features",
                    historySize: 1440, enforceNonNegativity: true);
        this.TrainPipeline = pipeline;
    }
}