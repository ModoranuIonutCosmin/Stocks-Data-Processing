using Stocks.General.Models.StocksInfoAggregates;
using Stocks.General.Models.TradeSuggestions;
using StocksFinalSolution.BusinessLogic.Features.Subscriptions.Strategy;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;
using StocksProccesing.Relational.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StocksFinalSolution.BusinessLogic.Features.TradeSuggestions
{
    public class TradeSuggestionsService : ITradeSuggestionsService
    {
        private readonly IStockPricesRepository _stockPricesRepository;

        public TradeSuggestionsService(IStockPricesRepository stockPricesRepository)
        {
            _stockPricesRepository = stockPricesRepository;
        }
        public async Task<List<TradeSuggestion>> DetermineViableTrades(ApplicationUser userRequesting, string ticker,
            string algorithm, TimeSpan interval)
        {
            var nextHorizonPrices = await _stockPricesRepository
                .GetPredictionsForTickerAndAlgorithmPaginated(ticker, algorithm, 0, 1000000);
            var results = new List<TradeSuggestion>();

            if (!nextHorizonPrices.Any())
            {
                return new List<TradeSuggestion>();
            }

            List<StocksPriceData> observations = new List<StocksPriceData>();

            StocksPriceData lastIncludedObservation = nextHorizonPrices.First();

            for (int i = 1; i < nextHorizonPrices.Count; i++)
            {
                if (lastIncludedObservation.Date.Add(interval) <= nextHorizonPrices[i].Date)
                {
                    lastIncludedObservation = nextHorizonPrices[i];
                    observations.Add(lastIncludedObservation);
                }
            }

            ViableTradesContext context = new ViableTradesContext();

            context.SetStrategy(ViableTradesStrategy.ShortTermBUY);

            var buyResults = await context.Execute(nextHorizonPrices);

            context.SetStrategy(ViableTradesStrategy.ShortTermSELL);

            var sellResults = await context.Execute(nextHorizonPrices);

            var investedAmount = 0.01m * userRequesting.Capital;

            results.AddRange(BuildTradeRequests(buyResults, isBuy: true, ticker, investedAmount));
            results.AddRange(BuildTradeRequests(sellResults, isBuy: false, ticker, investedAmount));

            return results;
        }


        private List<TradeSuggestion> BuildTradeRequests(List<StocksPriceData> observations, bool isBuy,
            string ticker, decimal investedAmount)
        {
            //TODO: Invested amount

            List<TradeSuggestion> transactions = new List<TradeSuggestion>();

            for (int observation = 0; observation < observations.Count - 1; observation += 2)
            {
                transactions.Add(new TradeSuggestion
                {
                    CurrentPrice = observations[observation].Price,
                    ExpectedPrice = observations[observation + 1].Price,
                    Ticker = ticker,

                    OpenRequest = new PlaceMarketOrderRequest
                    {
                        InvestedAmount = investedAmount,
                        IsBuy = isBuy,
                        Leverage = 1,
                        StopLossAmount = (investedAmount * 3) / 4m,
                        TakeProfitAmount = int.MaxValue,
                        Ticker = ticker,
                        ScheduledOpen = observations[observation].Date,
                        ScheduledClose = observations[observation + 1].Date,
                        Token = Guid.NewGuid().ToString()
                    }
                });
            }

            return transactions;
        }
    }
}
