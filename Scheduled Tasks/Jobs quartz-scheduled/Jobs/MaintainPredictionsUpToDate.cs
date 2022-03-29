using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Stocks.General;
using Stocks.General.ExtensionMethods;
using Stocks_Data_Processing.Actions;
using Stocks_Data_Processing.Interfaces.Jobs;
using StocksFinalSolution.BusinessLogic.Features.Transactions;
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
        private readonly IStockPricesRepository _stockPricesRepository;
        private readonly IStockSummariesRepository _summariesRepository;
        private readonly ICompaniesRepository _companiesRepository;
        private readonly IMaintainanceJobsRepository _jobsRepository;
        private readonly ILogger<MaintainPredictionsUpToDate> _logger;

        private static readonly TimeSpan DatasetInterval = TimeSpan.FromHours(1);

        private static readonly Dictionary<Type, string> TabularAlgorithms = new()
        {
            {typeof(TabularSdcaRegressionPredictionEngine),         "T_SDCA"},
            {typeof(TabularFastForestRegressionPredictionEngine),   "T_FFO"},
            {typeof(TabularFastTreeRegressionPredictionEngine),     "T_FTO"},
            {typeof(TabularLbfgsPoissonRegressionPredictionEngine), "T_LBFP"},
        };
        
        private static readonly Dictionary<Type, string> TimeseriesAlgorithms = new()
        {
            {typeof(SSAPredictionEngine), "TS_SSA"}
        };
            
        public MaintainPredictionsUpToDate(
            IStockSummariesRepository summariesRepository,
            IStockPricesRepository stockPricesRepository,
            ICompaniesRepository companiesRepository,
            IMaintainanceJobsRepository jobsRepository,
            ILogger<MaintainPredictionsUpToDate> logger)
        {
            _summariesRepository = summariesRepository;
            _stockPricesRepository = stockPricesRepository;
            _companiesRepository = companiesRepository;
            _jobsRepository = jobsRepository;
            _logger = logger;
        }


        public async Task Execute(IJobExecutionContext context)
        {
            await UpdatePredictionsAsync();
        }

        public async Task UpdatePredictionsAsync()
        {
            _logger.LogWarning("[Predictions refresh task] Started prediction refreshing! " +
                               $"{DateTimeOffset.UtcNow}");

            _companiesRepository.EnsureCompaniesDataExists();

            foreach (string ticker in TickersHelpers.GatherAllTickers())
            {
                var datasetFlat = await GatherTimeseriesDataset(ticker);
                IEnumerable<TabularModelInput> datasetTabular = Enumerable.Empty<TabularModelInput>();
                var latestKnownDate = datasetFlat.Last().Date;

                foreach (var algorithm in TimeseriesAlgorithms.Keys)
                {
                    var predictionEngine = Activator.CreateInstance(algorithm, datasetFlat) 
                        as IPredictionEngine;
                    
                    var predictions = await predictionEngine
                        .ComputePredictionsForNextPeriod(16 * 5, 0);

                    await UpdatePredictionsForAlgorithmAndTicker(latestKnownDate, TimeseriesAlgorithms[algorithm],
                        ticker, predictions);
                }

                if (datasetFlat.Count() < 80)
                {
                    continue;
                }

                datasetTabular = datasetFlat.Tabularize(16 * 5).
                    Select(set => new TabularModelInput
                    {
                        Features = set
                            .SkipLast(1)
                            .Select(e => e.Price)
                            .ToArray(),
                        Label = set
                            .Last()
                            .Price
                    });
                datasetFlat = Enumerable.Empty<TimestampPriceInputModel>();
                
                foreach (var algorithm in TabularAlgorithms.Keys)
                {
                    var predictionEngine = Activator.CreateInstance(algorithm, datasetTabular) 
                        as IPredictionEngine;
                    
                    var predictions = await predictionEngine
                        .ComputePredictionsForNextPeriod(16 * 5, 0);

                    await UpdatePredictionsForAlgorithmAndTicker(latestKnownDate, TabularAlgorithms[algorithm],
                        ticker, predictions);
                    
                }
            }

            _jobsRepository.MarkJobFinished(MaintainanceTasksSchedulerHelpers.PredictionsRefreshJob);
            _logger.LogWarning($"[Predictions maintan task] Done prediction refreshing! { DateTimeOffset.UtcNow }");
        }


        private async Task UpdatePredictionsForAlgorithmAndTicker(DateTimeOffset latestDatasetDate, string algorithm, 
            string ticker, List<PredictionResult> predictions)
        {
            var currentDate = latestDatasetDate;
            var nextStockPrices = predictions.Select(prediction =>
            {
                currentDate = new DateTimeOffset(currentDate
                    .GetNextStockMarketTime(DatasetInterval).Ticks, TimeSpan.Zero);

                return new StocksPriceData
                {
                    Prediction = true,
                    AlgorithmUsed = algorithm,
                    Price = (decimal) prediction.Price,
                    CompanyTicker = ticker,
                    Date = currentDate
                };
            }).ToList();
            
            await _stockPricesRepository.RemoveAllPricePredictionsForTickerAndAlgorithm(
                ticker, algorithm);
            await _stockPricesRepository.AddPricesDataAsync(nextStockPrices);
        }

        private async Task<IEnumerable<TimestampPriceInputModel>> GatherTimeseriesDataset(string ticker)
        {
            return
                (await _summariesRepository
                    .GetAllWhereAsync(p => p.CompanyTicker == ticker &&
                                           p.Period == DatasetInterval.Ticks)
                )
                .OrderBy(p => p.Date)
                .Select(price => new TimestampPriceInputModel
                {
                    Date = new DateTime(price.Date.Ticks, DateTimeKind.Utc),
                    Price = (float) price.CloseValue
                });
        }
    }
}
