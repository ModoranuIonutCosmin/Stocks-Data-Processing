using Microsoft.Extensions.Logging;
using Quartz;
using Stocks.General;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Extension_Methods;
using StocksProccesing.Relational.Model;
using StocksProcessing.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stocks_Data_Processing.Utilities
{
    public class MaintainPredictionsUpToDate : IMaintainPredictionsUpToDate, IJob
    {
        private readonly StocksMarketContext _stocksContext;
        private readonly IPredictionsService _predictionsService;
        private readonly ILogger<MaintainPredictionsUpToDate> _logger;

        #region Private members - Variables

        /// <summary>
        /// Tine intr-o variabila statica lista companiilor pe care le
        /// urmarim spre a le updata datele in BD.
        /// See creaza pornind de la toate field-urile enumeratiei <see cref="StocksTicker"/>
        /// </summary>
        private static readonly List<string> WatchList
            = Enum.GetValues(typeof(StocksTicker)).Cast<StocksTicker>()
                                                .Select(s => s.ToString()).ToList();
        #endregion

        public MaintainPredictionsUpToDate(StockContextFactory stockContextFactory,
            IPredictionsService predictionsService,
            ILogger<MaintainPredictionsUpToDate> logger)
        {
            _stocksContext = stockContextFactory.Create();
            _predictionsService = predictionsService;
            _logger = logger;
        }


        public async Task Execute(IJobExecutionContext context)
        {
            await UpdatePredictionsAsync();
        }

        public async Task UpdatePredictionsAsync()
        {
            var tasks = new List<Task<List<PredictionResult>>>();

            _logger.LogWarning($"[Predictions maintan task] Started prediction refreshing! {DateTimeOffset.UtcNow}");

            _stocksContext.EnsureCompaniesDataExists();

            foreach (var ticker in WatchList)
            {
                tasks.Add(GatherPredictions(ticker));
            }

            var results = (await Task.WhenAll(tasks)).SelectMany(x => x)
                .Select(e => new StocksPriceData()
                {
                    Price = (decimal)Math.Round(e.Price, 2),
                    CompanyTicker = e.Ticker,
                    Prediction = true,
                    Date = e.Date
                }).ToList();

            _stocksContext.PricesData.RemoveRange(_stocksContext.PricesData.Where(k => k.Prediction));
            await _stocksContext.PricesData.AddRangeAsync(results);
            await _stocksContext.SaveChangesAsync();

            _logger.LogWarning($"[Predictions maintan task] Done prediction refreshing! { DateTimeOffset.UtcNow }");
        }

        public async Task<List<PredictionResult>> GatherPredictions(string ticker)
        {
            var predictionsChunk = (await _predictionsService.Predict(ticker));

            var max = predictionsChunk.Max();
            var min = predictionsChunk.Min();


            if (max.Price > 100000 || min.Price <= 0)
            {
                _logger.LogError($"Bad values, Was {max.Price} for max and {min.Price} for min!");
                return new List<PredictionResult>();
            }
            else
            {
                _logger.LogWarning($"Values are {max.Price} for max and {min.Price} for min!");
            }

            _logger.LogWarning($"Updated predictions for {ticker}!");

            return predictionsChunk;
        }


    }
}
