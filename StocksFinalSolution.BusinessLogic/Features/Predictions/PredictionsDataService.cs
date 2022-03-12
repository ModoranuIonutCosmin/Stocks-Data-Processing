using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stocks.General;
using Stocks.General.Models;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;

namespace StocksFinalSolution.BusinessLogic.Features.Predictions
{
    public class PredictionsDataService : IPredictionsDataService
    {
        private readonly IStockPricesRepository _stockPricesRepository;

        private List<string> AvailablePredictionsEngines = new List<string>()
        {
            "T_SDCA",
            "T_FFO",
            "T_FTO",
            "T_LBFP",
            "TS_SSA",
        };

        public PredictionsDataService(IStockPricesRepository stockPricesRepository)
        {
            _stockPricesRepository = stockPricesRepository;
        }
        public async Task<StocksPredictionsPaginatedDTO> GatherPredictions(string predictionEngine,
            string ticker, int page, int count)
        {
            if (!AvailablePredictionsEngines.Contains(predictionEngine))
            {
                throw new InvalidOperationException($"Invalid algorithm {predictionEngine}");
            }
            if (!Enum.IsDefined(typeof(StocksTicker), ticker.ToUpper()))
            {
                throw new InvalidOperationException($"Invalid stock market  ticker {ticker}!");
            }

            var predictions = await _stockPricesRepository
                .GetPredictionsForTickerAndAlgorithmPaginated(ticker, predictionEngine, page, count);

            return new StocksPredictionsPaginatedDTO()
            {
                Predictions = predictions,
                Algorithm = predictionEngine,
                Count = predictions.Count,
                Page = page,
                Ticker = ticker,
                TotalCount = await _stockPricesRepository
                    .GetPredictionsCountForTickerAndAlgorithm(ticker, predictionEngine)
            };
        }
    }
}