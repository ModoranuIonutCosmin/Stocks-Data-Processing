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
        static async Task Main()
        {
            var ticker = "TSLA";

            MLContext mlContext = new();

            DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<PriceDataInputModel>();

            // string query = $"SELECT CAST(Date AS DateTime) as Date, CAST(Price AS REAL) as Price FROM " +
            //                $"PricesData WHERE CompanyTicker = '{ticker}' ORDER BY Date asc";
            string query = $"SELECT CAST(Date AS DateTime) as Date, CAST(CloseValue AS REAL) as Price FROM " +
            $"[dbo].[Summaries] WHERE CompanyTicker = '{ticker}' AND Period = 3000000000 ORDER BY Date asc";

            DatabaseSource dbSource = new(SqlClientFactory.Instance,
                                            DatabaseSettings.ConnectionString,
                                            query);

            IDataView dataView = loader.Load(dbSource);


            var allData = mlContext.Data.CreateEnumerable<PriceDataInputModel>(dataView, false);
            Console.WriteLine("starting to tabularize");
            var result = allData.Tabularize(1440);
            await result.WriteDatasetToFile("./dataset.csv");

            // var results = new PredictionEngine().EvaluateModel(allData, horizon: 144);

            // // var results = new PredictionEngine().Predict(ticker);

            // results.ForEach(k => Console.WriteLine(k));
        }
    }
}
