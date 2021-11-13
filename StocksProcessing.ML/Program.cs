using Microsoft.ML;
using Microsoft.ML.Data;
using StocksProccesing.Relational;
using StocksProcessing.ML.Models;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace StocksProcessing.ML
{
    static class Program
    {
        static void Main()
        {
            var ticker = "TSLA";

            MLContext mlContext = new();

            DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<PriceDataInputModel>();

            string query = $"SELECT CAST(Date AS DateTime) as Date, CAST(Price AS REAL) as Price FROM " +
                           $"PricesData WHERE CompanyTicker = '{ticker}' ORDER BY Date asc";

            DatabaseSource dbSource = new(SqlClientFactory.Instance,
                                            DatabaseSettings.ConnectionString,
                                            query);

            IDataView dataView = loader.Load(dbSource);

            var allData = mlContext.Data.CreateEnumerable<PriceDataInputModel>(dataView, false);

            //var results = new PredictionEngine().EvaluateModel(allData);

            var results = new PredictionEngine().Predict(ticker);

            results.ForEach(k => Console.WriteLine(k));
        }

    //    static async Task BenchmarkModel(string ticker)
    //    {
    //        string rootDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../"));
    //        string modelPath = Path.Combine(rootDir, "MLModell.zip");
    //        var connectionString = "Server=.;Database=stocksDb;Trusted_Connection=True;MultipleActiveResultSets=true";


    //        var separatedDataSet = allData.SeparateDataSet(0.1);
    //        var trainDataLength = separatedDataSet.TrainData.Count();
    //        var trainInput
    //            = mlContext.Data.LoadFromEnumerable<PriceDataInputModel>(separatedDataSet.TrainData);
    //        var testInput
    //            = mlContext.Data.LoadFromEnumerable<PriceDataInputModel>(separatedDataSet.TestData);

    //        //var rowNum = (int)firstYearData.GetRowCount();
    //        var forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
    //            outputColumnName: "ForecastedPrices",
    //            inputColumnName: "Price",
    //            windowSize: 60 * 24,
    //            seriesLength: trainDataLength,
    //            trainSize: trainDataLength,
    //            horizon: 3 * 24 * 60,
    //            confidenceLevel: 0.95f,
    //            confidenceLowerBoundColumn: "LowerBoundPrices",
    //            confidenceUpperBoundColumn: "UpperBoundPrices");

    //        SsaForecastingTransformer forecaster = forecastingPipeline.Fit(trainInput);

    //        var forecastEngine = forecaster.CreateTimeSeriesEngine<PriceDataInputModel, PredictedPriceOutputModel>(mlContext);
    //        forecastEngine.CheckPoint(mlContext, modelPath);

    //        Evaluate(testInput, 3 * 24 * 60, forecastEngine, mlContext);
    //    }

    //    static void Evaluate(IDataView testData, int horizon,
    //        TimeSeriesPredictionEngine<PriceDataInputModel, PredictedPriceOutputModel> forecaster, MLContext mlContext)
    //    {

    //        PredictedPriceOutputModel forecast = forecaster.Predict();

    //        var wholeData = mlContext.Data.CreateEnumerable<PriceDataInputModel>(testData, reuseRowObject: false)
    //                .Take(horizon).ToList();

    //        var actualData = wholeData.Select(p => p.Price).ToList();


    //        var predictedData = forecast.ForecastedPrices.ToList();


    //        // Output predictions
    //        Console.WriteLine("Rental Forecast");
    //        Console.WriteLine($"RMSE: {predictedData.CalculateRMSE(actualData)}, MAE: {predictedData.CalculateMAE(actualData)} ");
    //        Console.WriteLine("---------------------");

    //        for (int i = 0; i < forecast.ForecastedPrices.Length; i++)
    //        {
    //            Console.WriteLine($"Data: {wholeData[i].Date}");
    //            Console.WriteLine($"Actual: {wholeData[i].Price}");
    //            Console.WriteLine($"Predicted: {forecast.ForecastedPrices[i]}");
    //            Console.WriteLine($"Upper: {forecast.UpperBoundPrices[i]}");
    //            Console.WriteLine($"Lower: {forecast.LowerBoundPrices[i]}");
    //            Console.WriteLine("---------------------------");
    //        }
    //    }
    }
}
