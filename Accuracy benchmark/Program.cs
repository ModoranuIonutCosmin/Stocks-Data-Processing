﻿using System.Text.Json;
using Accuracy_benchmark;
using Microsoft.Data.SqlClient;
using Microsoft.ML;
using Microsoft.ML.Data;
using StocksProccesing.Relational;
using StocksProcessing.ML.Models.TimeSeries;

var predictionHorizon = TimeSpan.FromHours(80);
var intervalTimepoints = TimeSpan.FromMinutes(1);
var companyTicker = "TSLA";
double testFraction = 0.5;
int tabularWindowSize = 18 * 60;

var horizon = (int) Math.Floor(predictionHorizon.Divide(intervalTimepoints));

var mlContext = new MLContext();

var loader = mlContext.Data.CreateDatabaseLoader<TimestampPriceInputModel>();
var querySummaries = "SELECT CAST(Date AS DateTime) as Date, CAST(CloseValue AS REAL) as Price FROM " +
            $"[dbo].[Summaries] WHERE CompanyTicker = '{companyTicker}' AND Period = {intervalTimepoints.Ticks}" +
            "ORDER BY Date asc";

var queryWhole = "SELECT CAST(Date AS DateTime) as Date, CAST(Price AS REAL) as Price FROM " +
            $"[dbo].[PricesData] WHERE CompanyTicker = '{companyTicker}'" +
            "ORDER BY Date asc";

DatabaseSource dbSource = new(SqlClientFactory.Instance,
    Environment.GetEnvironmentVariable("DATABASE_URL_PROD") ?? DatabaseSettings.ConnectionString,
    queryWhole);

var dataView = loader.Load(dbSource);

var trainData 
    = mlContext.Data.CreateEnumerable<TimestampPriceInputModel>(dataView, false)
        .ToList();


var benchmarker = new AccuracyBenchmarker(trainData, tabularWindowSize, intervalTimepoints);

var benchmarkResults = new List<AccuracyBenchmarkResult>();

benchmarkResults.Add(await benchmarker.BenchmarkSingleSpectrumAnalysis(companyTicker, horizon, testFraction: testFraction));
benchmarkResults.Add(await benchmarker.BenchmarkSDCA(companyTicker, horizon, testFraction: testFraction));
benchmarkResults.Add(await benchmarker.BenchmarkFastForest(companyTicker, horizon, testFraction: testFraction));
benchmarkResults.Add(await benchmarker.BenchmarkFastTreeTweedie(companyTicker, horizon, testFraction: testFraction));


var jsonString = JsonSerializer.Serialize(benchmarkResults);

await File.WriteAllTextAsync("out.txt", jsonString);