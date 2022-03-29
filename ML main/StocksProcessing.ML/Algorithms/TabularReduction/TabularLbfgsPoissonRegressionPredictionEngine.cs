using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ML;
using StocksProcessing.ML.Algorithms.Base;
using StocksProcessing.ML.Models.Tabular;

namespace StocksProcessing.ML.Algorithms.TabularReduction;

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
            .LbfgsPoissonRegression(@"Label", @"Features",
                historySize: 80, enforceNonNegativity: true);
        TrainPipeline = pipeline;
    }
}