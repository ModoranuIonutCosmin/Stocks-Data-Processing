using System.Text.Json;
using Accuracy_benchmark;
using Microsoft.Data.SqlClient;
using Microsoft.ML;
using Microsoft.ML.Data;
using StocksProccesing.Relational;
using StocksProcessing.ML.Models.TimeSeries;


TimeSpan predictionHorizon = TimeSpan.FromHours(80);
TimeSpan intervalTimepoints = TimeSpan.FromMinutes(5);
string companyTicker = "VOO";

int horizon = (int)Math.Floor(predictionHorizon.Divide(intervalTimepoints));

var mlContext = new MLContext();

DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<TimestampPriceInputModel>();
string query = $"SELECT CAST(Date AS DateTime) as Date, CAST(CloseValue AS REAL) as Price FROM " +
               $"[dbo].[Summaries] WHERE CompanyTicker = '{companyTicker}' AND Period = {intervalTimepoints.Ticks}" +
               $"ORDER BY Date asc";

DatabaseSource dbSource = new(SqlClientFactory.Instance,
    Environment.GetEnvironmentVariable("DATABASE_URL_PROD") ?? DatabaseSettings.ConnectionString,
    query);

IDataView dataView = loader.Load(dbSource);

var trainData = mlContext.Data.CreateEnumerable<TimestampPriceInputModel>(dataView, false);


var benchmarker = new AccuracyBenchmarker(trainData.ToList(), 960, intervalTimepoints);

AccuracyBenchmarkResult benchmarkResult 
    = await benchmarker.BenchmarkSingleSpectrumAnalysis(companyTicker, horizon);
var jsonString = JsonSerializer.Serialize(benchmarkResult);

File.AppendAllText("out.txt", "\r\n======================\r\n");
File.AppendAllText("out.txt", jsonString);