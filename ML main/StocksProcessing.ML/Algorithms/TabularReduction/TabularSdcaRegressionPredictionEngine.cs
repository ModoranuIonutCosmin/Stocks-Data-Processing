using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ML;
using StocksProcessing.ML.Algorithms.Base;
using StocksProcessing.ML.Models.Tabular;

namespace StocksProcessing.ML.Algorithms.TabularReduction;

public class TabularSdcaRegressionPredictionEngine : TabularPredictionEngine
{
    public TabularSdcaRegressionPredictionEngine(IEnumerable<TabularModelInput> dataset) :
        base(dataset)
    {
    }

    public override async Task SetupPipeline(int horizon)
    {
        var pipeline = MlContext.Regression.Trainers
            .Sdca(@"Label", @"Features");
        TrainPipeline = pipeline;
    }
}