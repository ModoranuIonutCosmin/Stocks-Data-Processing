using Microsoft.ML;
using Microsoft.ML.Data;
using StocksProccesing.Relational;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using StocksProcessing.ML.Models.TimeSeries;

namespace StocksProcessing.ML
{
    static class Program
    {
        static async Task Main()
        {
            var ticker = "TSLA";

            MLContext mlContext = new();

            DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<TimestampPriceInputModel>();

            // string query = $"SELECT CAST(Date AS DateTime) as Date, CAST(Price AS REAL) as Price FROM " +
            //                $"PricesData WHERE CompanyTicker = '{ticker}' ORDER BY Date asc";
            string query = $"SELECT CAST(Date AS DateTime) as Date, CAST(CloseValue AS REAL) as Price FROM " +
            $"[dbo].[Summaries] WHERE CompanyTicker = '{ticker}' AND Period = 3000000000 ORDER BY Date asc";

            DatabaseSource dbSource = new(SqlClientFactory.Instance,
                                            DatabaseSettings.ConnectionString,
                                            query);

            IDataView dataView = loader.Load(dbSource);


            var allData = mlContext.Data.CreateEnumerable<TimestampPriceInputModel>(dataView, false);
            Console.WriteLine("starting to tabularize");
            var result = allData
                .Tabularize(1440)
                .Select(list => 
                    list.Select(datasetEntry => datasetEntry.Price)
                        .ToList())
                .ToList();
                
            await result.WriteDatasetToFile("./dataset.csv");

            // var results = new PredictionEngine().EvaluateModel(allData, horizon: 144);

            // // var results = new PredictionEngine().Predict(ticker);

            // results.ForEach(k => Console.WriteLine(k));
        }
    }
}
