using Microsoft.ML;
using Tabular_data_shift;
using TabularML;

var mlContext = new MLContext();
IDataView data = mlContext.Data
    .LoadFromTextFile<MLModel1.ModelInput>("dataset.csv", separatorChar: ',', hasHeader: false);

var trainData = mlContext.Data
    .CreateEnumerable<TabularModelInput>(data, reuseRowObject: false);

IPredictionEngine predictionEngine = new TabularFastTreeRegressionPredictionEngine(trainData);

var result = await predictionEngine.EvaluateModel(1440, 0.15);

Console.WriteLine(result);