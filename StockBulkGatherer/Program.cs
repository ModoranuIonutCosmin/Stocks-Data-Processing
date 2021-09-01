using Autofac;
using Stocks.General;
using Stocks_Data_Processing;
using Stocks_Data_Processing.Models;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Extension_Methods;
using StocksProccesing.Relational.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace StockBulkGatherer
{
    class Program
    {
        public static List<string> WatchList
            = Enum.GetValues(typeof(StocksTicker)).Cast<StocksTicker>()
                                    .Select(s => s.ToString()).ToList();
        private static readonly object syncLock = new object();
        public static HttpClient httpClient = new();
        public static StocksMarketContext _dbContext;


        static List<StocksPriceData> GetJsonEntries(string jsonText, string ticker)
        {
            List<StocksPriceData> items = new();

            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonText);

            foreach (var item in json.results)
            {
                items.Add(new StocksPriceData
                {
                    CompanyTicker = ticker,
                    Date = DateTimeOffset.FromUnixTimeSeconds((long)item.t / 1000).ToUniversalTime(),
                    Price = item.c,
                    Prediction = false
                });
            }

            return items;
        }

        static async Task GatherApiStockDataIntoDatabase(string ticker, string api_key, int chunksNo)
        {
            var items = new List<StocksPriceData>();
            var To = DateTimeOffset.UtcNow;
            var From = DateTimeOffset.UtcNow.AddMonths(-3);

            while (chunksNo > 0)
            {
                Console.WriteLine($"Calling for {ticker}");

                var builtLink = $"https://api.polygon.io/v2/aggs/ticker/{ticker}/range/1/minute/" +
                $"{From.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}/{To.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}" +
                $"?adjusted=true&sort=asc&limit=50000&apiKey={api_key}";
                var response = await httpClient.GetAsync(builtLink);

                response.EnsureSuccessStatusCode();

                var jsonReponse = await response.Content.ReadAsStringAsync();

                items.AddRange(GetJsonEntries(jsonReponse, ticker));

                To = From.AddDays(-1);
                From = To.AddMonths(-3);

                chunksNo--;
            }

            ApplyPostProcessing(items);

            lock (syncLock)
            {
                _dbContext.PricesData.AddRange(items);
            }

            items.Clear();
        }

        static void ApplyPostProcessing(List<StocksPriceData> items)
        {
            StocksPriceData current;
            StocksPriceData next;
            DateTimeOffset currentDt;
            DateTimeOffset nextDt;

            int newRows = 0;

            for (int i = 0; i < items.Count - 1; i++)
            {
                current = items[i];
                next = items[i + 1];
                currentDt = current.Date;
                nextDt = next.Date;

                if (currentDt.Day == nextDt.Day)
                {
                    TimeSpan deltaTime = nextDt - currentDt;
                    double source, target;

                    source = current.Price;
                    target = next.Price;

                    for (int j = 1; j < deltaTime.TotalMinutes; ++j)
                    {
                        ++i;

                        source = (source + target) / 2;
                        source = Math.Round(source, 2);

                        var fillingRow = new StocksPriceData()
                        {
                            CompanyTicker = current.CompanyTicker,
                            Price = source,
                            Date = currentDt.AddMinutes(j),
                            Prediction = false
                        };

                        items.Insert(i, fillingRow);

                        newRows++;
                    }
                }
            }

            Console.WriteLine($"Added new {newRows} rows!");
        }

        static async Task Main()
        {

            var DIContainer = DIContainerConfig.Configure();

            using var scope = DIContainer.BeginLifetimeScope();
            var application = scope.Resolve<IApplication>();


            _dbContext = scope.Resolve<StockContextFactory>().Create();

            //var ticker = "VOO";
            var api_key = "ItDQkGgz7847ipgJ_e11TpgPrSBDkVJr";
            var chunksNo = 1;
            var limitPerMinute = 5;

            _dbContext.EnsureCompaniesDataExists();

            var tasks = new List<Task>();

            foreach (var ticker in WatchList)
            {
                if (limitPerMinute == 0)
                {
                    await Task.Delay(TimeSpan.FromMinutes(1));
                }

                tasks.Add(GatherApiStockDataIntoDatabase(ticker, api_key, chunksNo));
                limitPerMinute--;
            }

            await Task.WhenAll(tasks);

            //using (StreamWriter writer = File.AppendText($"log_{ticker}.txt"))
            //{
            //    writer.Write(JsonSerializer.Serialize(items));
            //}

            _dbContext.SaveChanges();
            Debugger.Break();
        }
    }
}

