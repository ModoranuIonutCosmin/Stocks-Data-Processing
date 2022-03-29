using System.Text.Json;
using Accuracy_benchmark;
using Microsoft.Data.SqlClient;
using Microsoft.ML;
using Microsoft.ML.Data;
using StocksProccesing.Relational;
using StocksProcessing.ML.Models.TimeSeries;

var predictionHorizon = TimeSpan.FromHours(80);
var intervalTimepoints = TimeSpan.FromMinutes(5);
var companyTicker = "VOO";

var horizon = (int) Math.Floor(predictionHorizon.Divide(intervalTimepoints));

var mlContext = new MLContext();

var loader = mlContext.Data.CreateDatabaseLoader<TimestampPriceInputModel>();
var query = "SELECT CAST(Date AS DateTime) as Date, CAST(CloseValue AS REAL) as Price FROM " +
            $"[dbo].[Summaries] WHERE CompanyTicker = '{companyTicker}' AND Period = {intervalTimepoints.Ticks}" +
            "ORDER BY Date asc";

DatabaseSource dbSource = new(SqlClientFactory.Instance,
    Environment.GetEnvironmentVariable("DATABASE_URL_PROD") ?? DatabaseSettings.ConnectionString,
    query);

var dataView = loader.Load(dbSource);

var trainData = mlContext.Data.CreateEnumerable<TimestampPriceInputModel>(dataView, false);


var benchmarker = new AccuracyBenchmarker(trainData.ToList(), 960, intervalTimepoints);

var benchmarkResult
    = await benchmarker.BenchmarkSingleSpectrumAnalysis(companyTicker, horizon);
var jsonString = JsonSerializer.Serialize(benchmarkResult);

File.AppendAllText("out.txt", "\r\n======================\r\n");
File.AppendAllText("out.txt", jsonString);