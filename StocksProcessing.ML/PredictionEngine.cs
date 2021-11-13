using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using StocksProccesing.Relational;
using StocksProcessing.ML.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Stocks.General.ExtensionMethods;

namespace StocksProcessing.ML
{
    public class PredictionEngine
    {
        private MLContext mlContext;
        public PredictionEngine()
        {
            mlContext = new MLContext();
        }

        public List<PredictionResult> Predict(string ticker, int horizon = 14400)
        {
            IEnumerable<PriceDataInputModel> stockData = LoadDataset(ticker);

            //Se converteste in BD cu CAST(DateTimeOffset -> Datetime) apoi aici inapoi la DateTimeOffset
            //Va avea offset +3 din oarece motiv chiar daca in BD e UTC(offset +0). Se modifica la loc.

            DateTime dateGMT3 = stockData.Last().Date;

            dateGMT3 = DateTime.SpecifyKind(dateGMT3, DateTimeKind.Utc);

            DateTimeOffset nextPredictionDateTime = dateGMT3;

            var predictionModel = TrainModel(stockData, horizon);

            var results = predictionModel.Predict();

            return results.ForecastedPrices.Select( (price, index) =>
            {
                nextPredictionDateTime = nextPredictionDateTime.GetNextStockMarketTime(TimeSpan.FromMinutes(1));

                var result = new PredictionResult()
                {
                    Price = price,
                    Date = nextPredictionDateTime,
                    Ticker = ticker,
                    LowerBoundPrice = results.LowerBoundPrices[index],
                    UpperBoundPrice = results.UpperBoundPrices[index],
                };

                return result;

            }).ToList();
        }

        public List<SingleTimepointPredictionComparePair> EvaluateModel(
        IEnumerable<PriceDataInputModel> data, double testFraction = 0.2,
        int horizon = 14400)
        {
            var splitData = data.SeparateDataSet(testFraction);

            var trainedModel = TrainModel(splitData.TrainData, horizon);

            PredictedPriceOutputModel forecast = trainedModel.Predict();

            var actualValuesData = splitData.TestData.Take(horizon).ToList();
            var actualValues = actualValuesData.Select(e => e.Price).ToList();

            Console.WriteLine($"RMSE: {forecast.ForecastedPrices.ToList().CalculateRMSE(actualValues)}");
            Console.WriteLine($"MAE: {forecast.ForecastedPrices.ToList().CalculateMAE(actualValues)}");

            return forecast.ForecastedPrices.Select(
                (e, index) => new SingleTimepointPredictionComparePair()
                {
                    DateTime = actualValuesData[index].Date,
                    ActualValue = actualValues[index],
                    PredictedValue = e
                }).ToList();
        }

        IEnumerable<PriceDataInputModel> LoadDataset(string ticker)
        {
            DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<PriceDataInputModel>();

            string query = $"SELECT CAST(Date AS DateTime) as Date, CAST(Price AS REAL) as Price FROM " +
                           $"PricesData WHERE CompanyTicker = '{ticker}' AND Prediction = 0 ORDER BY Date asc";
            DatabaseSource dbSource = new DatabaseSource(SqlClientFactory.Instance,
                                            DatabaseSettings.ConnectionString,
                                            query);

            IDataView dataView = loader.Load(dbSource);

            var allData = mlContext.Data.CreateEnumerable<PriceDataInputModel>(dataView, false);

            return allData;
        }



        public TimeSeriesPredictionEngine<PriceDataInputModel, PredictedPriceOutputModel> TrainModel(
            IEnumerable<PriceDataInputModel> dataset, int horizon)
        {
            var stockDataView = mlContext.Data.LoadFromEnumerable(dataset);
            var trainDataLength = dataset.Count();

            var forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
               outputColumnName: "ForecastedPrices",
               inputColumnName: "Price",
               windowSize: 5 * 16 * 60,
               seriesLength: trainDataLength,
               trainSize: trainDataLength,
               horizon: horizon,
               confidenceLevel: 0.95f,
               confidenceLowerBoundColumn: "LowerBoundPrices",
               confidenceUpperBoundColumn: "UpperBoundPrices");

            var forecaster = forecastingPipeline.Fit(stockDataView);


            var forecastEngine = forecaster.CreateTimeSeriesEngine<PriceDataInputModel,
                                                                    PredictedPriceOutputModel>(mlContext);

            return forecastEngine;
        }


    }
}
