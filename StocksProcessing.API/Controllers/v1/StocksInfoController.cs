using Microsoft.AspNetCore.Mvc;
using Stocks.General;
using Stocks.General.ExtensionMethods;
using Stocks.General.Models;
using StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator;
using StocksProccesing.Relational.DataAccess.V1.Repositories;
using StocksProccesing.Relational.Model;
using StocksProcessing.API.Models;
using StocksProcessing.API.Payloads;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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
        private readonly IStockMarketDisplayPriceCalculator priceCalculator;
        private readonly IStocksTrendCalculator stocksTrendCalculator;

        public StocksInfoController(IStockSummariesRepository stockSummariesRepository,
            ICompaniesRepository companiesRepository,
            IStockPricesRepository stockPricesRepository,
            IStocksTrendCalculator stocksTrendCalculator,
            IStockMarketDisplayPriceCalculator priceCalculator)
        {
            this.stockSummariesRepository = stockSummariesRepository;
            this.companiesRepository = companiesRepository;
            this.stockPricesRepository = stockPricesRepository;
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
                        BuyPrice = buyPrice,
                        SellPrice = sellPrice,
                        Trend = trend,
                        Name = company.Name,
                        Ticker = company.Ticker,
                        Description = company.Description,
                        UrlLogo = company.UrlLogo,
                        Period = TimeSpan.FromDays(1).Ticks,
                        Timepoint = new OHLCPriceValue
                        {
                            CloseValue = summary.CloseValue,
                            Date = summary.Date,
                            High = summary.High,
                            Low = summary.Low,
                            OpenValue = summary.OpenValue
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
            Company company = companiesRepository.GetCompanyData(ticker);
            long intervalTicks = TimespanParser.ParseTimeSpanTicks(interval);
            var dataPoints = (await stockSummariesRepository
                    .GetAllWhereAsync(p => p.CompanyTicker == ticker && p.Period == intervalTicks))
                    .Select(e => new OHLCPriceValue()
                    {
                        CloseValue = e.CloseValue,
                        Date = e.Date,
                        High = e.High,
                        Low = e.Low,
                        OpenValue = e.OpenValue
                    }).ToList();

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
        public async Task<ApiResponse<AllStocksPricePredictionsModel>> GetPredictions
                                                        ([NotNull] string ticker)
        {
            var response = new ApiResponse<AllStocksPricePredictionsModel>();

            if (!Enum.IsDefined(typeof(StocksTicker), ticker.ToUpper()))
            {
                response.ErrorMessage = "Provide a known stock market ticker!";
                Response.StatusCode = (int)HttpStatusCode.NotFound;

                return response;
            }

            Company companyInfo;

            try
            {
                companyInfo = await companiesRepository
                    .GetPredictionsByTicker(ticker);

                if (companyInfo == null)
                    throw new Exception("No company goes by that name!");
            }

            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;

                return response;
            }

            var result = new AllStocksPricePredictionsModel
            {
                Ticker = companyInfo.Ticker,
                Name = companyInfo.Name,
                Description = companyInfo.Description,
                UrlLogo = companyInfo.UrlLogo,
                Predictions = companyInfo.PricesData
                            .Select(e => new TimestampPrices()
                            {
                                Date = e.Date,
                                Prediction = true,
                                Price = e.Price
                            }).ToList()
            };

            response.Response = result;

            return response;
        }

    }
}
