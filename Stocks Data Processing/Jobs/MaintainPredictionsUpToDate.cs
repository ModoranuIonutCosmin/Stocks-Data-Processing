using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Stocks.General;
using Stocks.General.ExtensionMethods;
using Stocks_Data_Processing.Actions;
using Stocks_Data_Processing.Interfaces.Jobs;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksProccesing.Relational.Model;
using StocksProcessing.ML;
using StocksProcessing.ML.Algorithms.Base;
using StocksProcessing.ML.Algorithms.TabularReduction;
using StocksProcessing.ML.Algorithms.TimeSeries;
using StocksProcessing.ML.Models.Tabular;
using StocksProcessing.ML.Models.TimeSeries;

namespace Stocks_Data_Processing.Jobs
{
    public class MaintainPredictionsUpToDate : IMaintainPredictionsUpToDate
    {
        private readonly IStockPricesRepository stockPricesRepository;
        private readonly ICompaniesRepository companiesRepository;
        private readonly IMaintainanceJobsRepository jobsRepository;
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

        private static readonly Dictionary<Type, string> Algorithms = new()
        {
            {typeof(TabularSdcaRegressionPredictionEngine),         "T_SDCA"},
            {typeof(TabularFastForestRegressionPredictionEngine),   "T_FFO"},
            {typeof(TabularFastTreeRegressionPredictionEngine),     "T_FTO"},
            {typeof(TabularLbfgsPoissonRegressionPredictionEngine), "T_LBFP"},
            {typeof(SSAPredictionEngine),                           "TS_SSA"}
        };
            
        private Dictionary<string, IPredictionEngine> PredictionEngines;
        #endregion

        public MaintainPredictionsUpToDate(
            IStockPricesRepository stockPricesRepository,
            ICompaniesRepository companiesRepository,
            IMaintainanceJobsRepository jobsRepository,
            ILogger<MaintainPredictionsUpToDate> logger)
        {
            this.stockPricesRepository = stockPricesRepository;
            this.companiesRepository = companiesRepository;
            this.jobsRepository = jobsRepository;
            _logger = logger;
        }


        public async Task Execute(IJobExecutionContext context)
        {
            await UpdatePredictionsAsync();
        }

        public async Task UpdatePredictionsAsync()
        {
            _logger.LogWarning($"[Predictions refresh task] Started prediction refreshing! " +
                               $"{DateTimeOffset.UtcNow}");

            companiesRepository.EnsureCompaniesDataExists();

            var trainingContexts = from ticker in WatchList
                from type in Algorithms.Keys
                select (ticker, type);
                
            
            foreach (var predictionParams in trainingContexts)
            {
                bool tabular = predictionParams.type.IsSubclassOf(typeof(TabularPredictionEngine));
                var datasetFlat = await GatherTimeseriesDataset(predictionParams.ticker);
                IEnumerable<TabularModelInput> datasetTabular = Enumerable.Empty<TabularModelInput>();
                var currentDate = new DateTimeOffset(datasetFlat.Last().Date.Ticks, TimeSpan.Zero);
                
                if (tabular)
                {
                    datasetTabular = datasetFlat.Tabularize(960)
                        .Select(obs =>
                            new TabularModelInput()
                            {
                                Features = obs.SkipLast(1).ToArray(),
                                Next = obs.Last()
                            });
                }

                var predictionEngine = Activator.CreateInstance(predictionParams.type, 
                    tabular ? datasetTabular : datasetFlat) as IPredictionEngine;
                var predictions = await predictionEngine
                    .ComputePredictionsForNextPeriod(12 * 16 * 5, 0);
                await stockPricesRepository.RemoveAllPricePredictionsForTickerAndAlgorithm(
                    predictionParams.ticker, Algorithms[predictionParams.type]);
                await stockPricesRepository.AddPricesDataAsync(
                    predictions.Select(prediction =>
                    {
                        currentDate = new DateTimeOffset(currentDate
                            .GetNextStockMarketTime(TimeSpan.FromMinutes(5)).Ticks, TimeSpan.Zero);

                        return new StocksPriceData()
                        {
                            Prediction = true,
                            AlgorithmUsed = Algorithms[predictionParams.type],
                            Price = (decimal) prediction.Price,
                            CompanyTicker = predictionParams.ticker,
                            Date = currentDate
                        };
                    }).ToList());
            }

            jobsRepository.MarkJobFinished(MaintainanceTasksSchedulerHelpers.PredictionsRefreshJob);
            _logger.LogWarning($"[Predictions maintan task] Done prediction refreshing! { DateTimeOffset.UtcNow }");
        }


        private async Task<IEnumerable<TimestampPriceInputModel>> GatherTimeseriesDataset(string ticker)
        {
            return
                (await stockPricesRepository
                    .GetAllWhereAsync(p => p.CompanyTicker == ticker &&
                                           p.Prediction == false)
                )
                .OrderBy(p => p.Date)
                .Select(price => new TimestampPriceInputModel()
                {
                    Date = new DateTime(price.Date.Ticks, DateTimeKind.Utc),
                    Price = (float) price.Price
                });
        }
    }
}
