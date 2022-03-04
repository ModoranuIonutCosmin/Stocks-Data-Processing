using Microsoft.ML;
using Microsoft.ML.Trainers.FastTree;
using TabularML.Algorithms.Base;

namespace TabularML.Algorithms.TabularReduction;

public class TabularFastForestRegressionPredictionEngine : TabularPredictionEngine
{
    public TabularFastForestRegressionPredictionEngine(IEnumerable<TabularModelInput> dataset) : 
        base(dataset)
    {
    }
    public override async Task SetupPipeline(int horizon)
    {
        string featuresColumnName = "Features";
        // Data process configuration with pipeline data transformations
        var pipeline = MlContext.Regression.Trainers
            .FastForest(new FastForestRegressionTrainer.Options()
            {
                NumberOfTrees = 4,
                FeatureFraction = 0.8F, LabelColumnName = @"Label", FeatureColumnName = @"Features",
            });

        // var pipeline = MlContext.Regression.Trainers
        //         .LbfgsPoissonRegression(labelColumnName: @"Label", featureColumnName: @"Features",
        //             historySize: 1440, enforceNonNegativity: true);
        // this.TrainPipeline = pipeline;


        // var pipeline = MlContext.Regression.Trainers
        //         .Gam(labelColumnName: @"Label", featureColumnName: @"Features");

        // var pipeline = MlContext.Regression.Trainers
        //         .FastTreeTweedie(labelColumnName: @"Label", featureColumnName: @"Features");

        // var pipeline = MlContext.Regression.Trainers
        //     .FastTree(labelColumnName: @"Label", featureColumnName: @"Features", learningRate:0.3f,
        //         numberOfLeaves: 20, numberOfTrees:100);
        this.TrainPipeline = pipeline;
    }
}