using Microsoft.Data.SqlClient;
using Microsoft.ML;
using Microsoft.ML.Data;
using StocksProccesing.Relational;
using StocksProcessing.ML.Algorithms.Base;
using StocksProcessing.ML.Algorithms.TimeSeries;
using StocksProcessing.ML.Models.TimeSeries;


var mlContext = new MLContext();

DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<TimestampPriceInputModel>();

string query = $"SELECT CAST(Date AS DateTime) as Date, CAST(CloseValue AS REAL) as Price FROM " +
               $"[dbo].[Summaries] WHERE CompanyTicker = 'TSLA' AND Period = 3000000000 ORDER BY Date asc";
DatabaseSource dbSource = new(SqlClientFactory.Instance,
    DatabaseSettings.ConnectionString,
    query);

IDataView dataView = loader.Load(dbSource);

var trainData = mlContext.Data.CreateEnumerable<TimestampPriceInputModel>(dataView, false);

IPredictionEngine predictionEngine = new SSAPredictionEngine(trainData);


var result = await predictionEngine.EvaluateModel(80, 0.15);

