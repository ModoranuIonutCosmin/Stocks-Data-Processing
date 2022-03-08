using Microsoft.Data.SqlClient;
using Microsoft.ML;
using Microsoft.ML.Data;
using StocksProccesing.Relational;
using StocksProcessing.ML.Models;
using Tabular_data_shift;
using TabularML;
using TabularML.Algorithms.Base;
using TabularML.Algorithms.TabularReduction;
using TabularML.Algorithms.TimeSeries;

var mlContext = new MLContext();


// IDataView data = mlContext.Data
//     .LoadFromTextFile<MLModel1.ModelInput>("dataset.csv", separatorChar: ',', hasHeader: false);
//
// var trainData = mlContext.Data
//     .CreateEnumerable<TabularModelInput>(data, reuseRowObject: false);
// IPredictionEngine predictionEngine = new TabularFastTreeRegressionPredictionEngine(trainData);


///
DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<TimestampPriceInputModel>();

string query = $"SELECT CAST(Date AS DateTime) as Date, CAST(CloseValue AS REAL) as Price FROM " +
               $"[dbo].[Summaries] WHERE CompanyTicker = 'TSLA' AND Period = 3000000000 ORDER BY Date asc";
DatabaseSource dbSource = new(SqlClientFactory.Instance,
    DatabaseSettings.ConnectionString,
    query);

IDataView dataView = loader.Load(dbSource);

var trainData = mlContext.Data.CreateEnumerable<TimestampPriceInputModel>(dataView, false);

IPredictionEngine predictionEngine = new SSAPredictionEngine(trainData);
///
/// 
var result = await predictionEngine.EvaluateModel(960, 0.15);

Console.WriteLine(result);