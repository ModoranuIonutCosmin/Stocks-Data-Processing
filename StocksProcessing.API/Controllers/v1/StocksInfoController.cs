﻿using Microsoft.AspNetCore.Mvc;
using Stocks.General.ExtensionMethods;
using Stocks.General.Models;
using StocksProccesing.Relational.Model;
using StocksProcessing.API.Models;
using StocksProcessing.API.Payloads;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;
using StocksFinalSolution.BusinessLogic.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StocksProcessing.API.Controllers.v1
{
    [ApiVersion("1.0")]
    [ApiController]
    public class StocksInfoController : BaseController
    {
        private readonly IStockSummariesRepository stockSummariesRepository;
        private readonly ICompaniesRepository companiesRepository;
        private readonly IStockPricesRepository stockPricesRepository;
        private readonly IPredictionsDataService _predictionsDataService;
        private readonly IStockMarketDisplayPriceCalculator priceCalculator;
        private readonly IStocksTrendCalculator stocksTrendCalculator;

        public StocksInfoController(IStockSummariesRepository stockSummariesRepository,
            ICompaniesRepository companiesRepository,
            IStockPricesRepository stockPricesRepository,
            IPredictionsDataService predictionsDataService,
            IStocksTrendCalculator stocksTrendCalculator,
            IStockMarketDisplayPriceCalculator priceCalculator)
        {
            this.stockSummariesRepository = stockSummariesRepository;
            this.companiesRepository = companiesRepository;
            this.stockPricesRepository = stockPricesRepository;
            _predictionsDataService = predictionsDataService;
            this.stocksTrendCalculator = stocksTrendCalculator;
            this.priceCalculator = priceCalculator;
        }


        [HttpGet("report")]
        public async Task<ApiResponse<List<StockReportSingle>>> GetReportsAllCompanies()
        {
            var companies = await companiesRepository.GetAllAsync();
            var summaries = stockSummariesRepository.GetLastSummaryEntryForAll(TimeSpan.FromDays(1));

            var result = companies.Join(summaries, c => c.Ticker, s => s.CompanyTicker,
                (company, summary) =>
                {
                    var trend = stocksTrendCalculator.CalculateTrendFromOHLC(summary);
                    var price = stockPricesRepository.GetCurrentUnitPriceByStocksCompanyTicker(company.Ticker);
                    var sellPrice = priceCalculator.CalculateSellPrice(price);
                    var buyPrice = priceCalculator.CalculateBuyPrice(price, 1);

                    return new StockReportSingle()
                    {
                        BuyPrice = buyPrice.TruncateToDecimalPlaces(3),
                        SellPrice = sellPrice.TruncateToDecimalPlaces(3),
                        Trend = trend.TruncateToDecimalPlaces(3),
                        Name = company.Name,
                        Ticker = company.Ticker,
                        Description = company.Description,
                        UrlLogo = company.UrlLogo,
                        Period = TimeSpan.FromDays(1).Ticks,
                        Timepoint = new OhlcPriceValue
                        {
                            Date = summary.Date,
                            High = summary.High,
                            Low = summary.Low,
                            OpenValue = summary.OpenValue,
                            CloseValue = summary.CloseValue,
                        }
                    };
                }).ToList();

            return new()
            {
                Response = result
            };
        }


        [HttpGet("historicalData")]
        public async Task<ApiResponse<StocksSummary>> GetHistoricalData([NotNull]string ticker, [NotNull]string interval)
        {
            if (string.IsNullOrEmpty(ticker))
            {
                throw new ArgumentException($"'{nameof(ticker)}' cannot be null or empty.", nameof(ticker));
            }

            if (string.IsNullOrEmpty(interval))
            {
                throw new ArgumentException($"'{nameof(interval)}' cannot be null or empty.", nameof(interval));
            }

            Company company = companiesRepository.GetCompanyData(ticker);
            long intervalTicks = TimespanParser.ParseTimeSpanTicks(interval);
            var dataPoints = (await stockSummariesRepository
                    .GetAllWhereAsync(p => p.CompanyTicker == ticker && p.Period == intervalTicks))
                    .Select(e => new OhlcPriceValue()
                    {
                        CloseValue = e.CloseValue,
                        Date = e.Date,
                        High = e.High,
                        Low = e.Low,
                        OpenValue = e.OpenValue
                    })
                    .OrderBy(e => e.Date)
                    .ToList();

            var trend = dataPoints.Count == 0 ? 0m
                : stocksTrendCalculator.CalculateTrendFromOHLC(dataPoints[^1]);
            var currentBasePrice = stockPricesRepository.GetCurrentUnitPriceByStocksCompanyTicker(ticker);

            return new ApiResponse<StocksSummary>()
            {
                Response = new StocksSummary()
                {
                    Period = intervalTicks,
                    UrlLogo = company.UrlLogo,
                    Name = company.Name,
                    Description = company.Description,
                    Ticker = ticker,
                    Trend = trend,
                    SellPrice = priceCalculator.CalculateSellPrice(currentBasePrice),
                    BuyPrice = priceCalculator.CalculateBuyPrice(currentBasePrice, 1),
                    Timepoints = dataPoints
                }
            };
        }

        [HttpGet("forecastData")]
        public async Task<ApiResponse<StocksPredictionsPaginatedDTO>> GetPredictions
                                                        ([NotNull] string ticker,
                                                            string algorithm = "T_FTO",
                                                            int page = 0, int count = 1000)
        {
            if (string.IsNullOrWhiteSpace(ticker))
            {
                throw new ArgumentException($"'{nameof(ticker)}' cannot be null or whitespace.", nameof(ticker));
            }

            return new ApiResponse<StocksPredictionsPaginatedDTO>()
            {
                Response = await _predictionsDataService.GatherPredictions(algorithm,
                    ticker, page, count)
            };

        }

    }
}
