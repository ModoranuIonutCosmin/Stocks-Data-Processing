using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stocks.General.Entities;
using Stocks.General.ExtensionMethods;
using Stocks.General.Models.StocksInfoAggregates;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Features.StocksMarketSummaryGenerator;

public class StocksSummaryGenerator : IStocksSummaryGenerator
{
    private readonly ICompaniesRepository companiesRepository;
    private readonly IStockMarketDisplayPriceCalculator priceCalculator;
    private readonly IStockPricesRepository stockPricesRepository;
    private readonly IStocksTrendCalculator stocksTrendCalculator;
    private readonly IStockSummariesRepository stockSummariesRepository;


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
        DateTimeOffset lastUpdatedDate = stockSummariesRepository
                                  .GetLastSummaryEntry(ticker, interval)?.Date
                              ?? DateTimeOffset.FromUnixTimeMilliseconds(0);

        List<StocksPriceData> pricesData = (await stockPricesRepository
                .GetAllWhereAsync(e => e.CompanyTicker == ticker &&
                                       e.Date > lastUpdatedDate.Add(interval) && !e.Prediction))
            .OrderBy(e => e.Date)
            .ToList();

        List<List<StocksPriceData>> groupedChunks = new List<List<StocksPriceData>>();

        if (!pricesData.Any())
            return new StocksSummary();

        DateTimeOffset currentTimePoint = pricesData.First().Date;
        List<StocksPriceData> currentGroup = new List<StocksPriceData>();

        foreach (var timePoint in pricesData)
        {
            if (timePoint.Date.Subtract(currentTimePoint) > interval)
            {
                groupedChunks.Add(currentGroup);
                currentGroup = new List<StocksPriceData>();
                currentTimePoint = timePoint.Date;
            }

            currentGroup.Add(timePoint);
        }

        groupedChunks.Add(currentGroup);

        var currentPrice = currentGroup.Last().Price;
        var companyInfo = companiesRepository.GetCompanyData(ticker);

        return new StocksSummary
        {
            Ticker = ticker,
            UrlLogo = companyInfo.UrlLogo,
            Name = companyInfo.Name,
            Description = companyInfo.Description,
            Period = interval.Ticks,
            Trend = stocksTrendCalculator.CalculateTrendFromList(groupedChunks.Last()),
            SellPrice = priceCalculator.CalculateSellPrice(currentPrice).TruncateToDecimalPlaces(3),
            BuyPrice = priceCalculator.CalculateBuyPrice(currentPrice).TruncateToDecimalPlaces(3),
            Timepoints = groupedChunks.Select(e => new OhlcPriceValue
            {
                CloseValue = e.Last().Price.TruncateToDecimalPlaces(3),
                OpenValue = e.First().Price.TruncateToDecimalPlaces(3),
                Date = e.First().Date,
                High = e.Max(k => k.Price).TruncateToDecimalPlaces(3),
                Low = e.Min(k => k.Price).TruncateToDecimalPlaces(3)
            }).ToList()
        };
    }
}