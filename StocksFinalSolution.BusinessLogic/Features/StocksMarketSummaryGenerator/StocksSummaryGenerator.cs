using Stocks.General.ExtensionMethods;
using Stocks.General.Models;
using StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator;
using StocksProccesing.Relational.Model;
using StocksProcessing.API.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;

namespace StocksFinalSolution.BusinessLogic.StocksMarketSummaryGenerator
{
    public class StocksSummaryGenerator : IStocksSummaryGenerator
    {
        private readonly ICompaniesRepository companiesRepository;
        private readonly IStockSummariesRepository stockSummariesRepository;
        private readonly IStockPricesRepository stockPricesRepository;
        private readonly IStocksTrendCalculator stocksTrendCalculator;
        private readonly IStockMarketDisplayPriceCalculator priceCalculator;


        
        public StocksSummaryGenerator(ICompaniesRepository companiesRepository,
            IStockSummariesRepository stockSummariesRepository,
            IStockPricesRepository stockPricesRepository,
            IStocksTrendCalculator stocksTrendCalculator,
            IStockMarketDisplayPriceCalculator priceCalculator)
        {
            this.companiesRepository = companiesRepository;
            this.stockSummariesRepository = stockSummariesRepository;
            this.stockPricesRepository = stockPricesRepository;
            this.stocksTrendCalculator = stocksTrendCalculator;
            this.priceCalculator = priceCalculator;
        }

        public async Task<StocksSummary> GenerateSummary(string ticker, TimeSpan interval)
        {
            var lastUpdatedDate = stockSummariesRepository
                .GetLastSummaryEntry(ticker, interval)?.Date 
                                                      ?? DateTimeOffset.FromUnixTimeMilliseconds(0);

            var pricesData = (await stockPricesRepository
                .GetAllWhereAsync(e => e.CompanyTicker == ticker && e.Date > lastUpdatedDate && !e.Prediction))
                .OrderByDescending(e => e.Date)
                .ToList();

            var companyInfo = companiesRepository.GetCompanyData(ticker);

            List<List<StocksPriceData>> groupedChunks = new List<List<StocksPriceData>>();

            if (!pricesData.Any())
                return new StocksSummary();

            var currentTimePoint = pricesData.First().Date;
            var currentGroup = new List<StocksPriceData>();

            foreach (var timePoint in pricesData)
            {
                if (currentTimePoint.Subtract(timePoint.Date) > interval)
                {
                    groupedChunks.Add(currentGroup);
                    currentGroup = new List<StocksPriceData>();
                    currentTimePoint = timePoint.Date;
                }

                currentGroup.Add(timePoint);
            }

            groupedChunks.Add(currentGroup);

            var currentPrice = currentGroup.Last().Price;
            return new StocksSummary()
            {
                Ticker = ticker,
                UrlLogo = companyInfo.UrlLogo,
                Name = companyInfo.Name,
                Description = companyInfo.Description,
                Period = interval.Ticks,
                Trend = stocksTrendCalculator.CalculateTrendFromList(groupedChunks.Last()),
                SellPrice = priceCalculator.CalculateSellPrice(currentPrice).TruncateToDecimalPlaces(3),
                BuyPrice = priceCalculator.CalculateBuyPrice(currentPrice, 1).TruncateToDecimalPlaces(3),
                Timepoints = groupedChunks.Select(e => new OhlcPriceValue()
                {
                    CloseValue = e.First().Price.TruncateToDecimalPlaces(3),
                    OpenValue = e.Last().Price.TruncateToDecimalPlaces(3),
                    Date = e.First().Date,
                    High = e.Max(k => k.Price).TruncateToDecimalPlaces(3),
                    Low = e.Min(k => k.Price).TruncateToDecimalPlaces(3)
                }).ToList()
            };
        }

    }
}
