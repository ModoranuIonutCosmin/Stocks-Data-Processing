using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StocksProccesing.Relational.DataAccess;
using StocksProcessing.API.Models;
using StocksProccesing.Relational.Extension_Methods;
using StocksProcessing.API.Payloads;
using StocksProccesing.Relational.Model;
using Stocks.General;
using Stocks.General.ExtensionMethods;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using StocksProcessing.API.Auth;
using Stocks.General.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StocksProcessing.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StocksInfoController : ControllerBase
    {
        private readonly StocksMarketContext _dbContext;

        public StocksInfoController(StocksMarketContext context)
        {
            _dbContext = context;
        }

        [HttpGet("companyInfo/{ticker}")]
        public async Task<ApiResponse<Company>> GetDescriptionByTicker(string ticker)
        {
            var response = new ApiResponse<Company>();

            try
            {
                var companyList = await _dbContext.Companies
                    .Where(e => e.Ticker.ToUpper() == ticker)
                    .AsNoTracking().ToListAsync();

                if(companyList.Count == 0)
                {
                    throw new Exception("Couldn't find company data!");
                }

                response.Response = companyList.First();
            }

            catch(Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        [HttpGet("report")]
        public async Task<ApiResponse<IList<StocksCurrentDaySummary>>> GetReportsAllCompanies()
        {
            var fromDate = DateTimeOffset.UtcNow.AddDays(-30).SetTime(8, 0);

            var response = new ApiResponse<IList<StocksCurrentDaySummary>>();

            try
            {

                var result = await _dbContext
                    .LoadStoredProcedure("dbo.spGetDailyStockSummary")
                    .WithSqlParams(
                    (nameof(fromDate), fromDate))
                    .ExecuteStoredProcedureAsync<StocksCurrentDaySummary>();

                response.Response = result;
            }

            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }

            return response;

            //.Join(_dbContext.Companies,
            //priceData => priceData.CompanyTicker,
            //companyData => companyData.Ticker,
            //(priceData, companyData) =>
            //new StocksDailySummaryModel()
            //{
            //    Ticker = priceData.CompanyTicker,
            //    UrlLogo = companyData.UrlLogo,
            //    Name = companyData.Name,
            //    Date = priceData.Date,
            //    Price = priceData.Price,
            //})

        }


        [HttpGet("report/{ticker}")]
        public async Task<ApiResponse<StocksCurrentDaySummary>> GetReportsByCompany([NotNull] string ticker)
        {
            //TODO: Remove this
            var fromDate = DateTimeOffset.UtcNow.AddDays(-6).SetTime(8, 0);

            var response = new ApiResponse<StocksCurrentDaySummary>();

            if (!Enum.IsDefined(typeof(StocksTicker), ticker))
            {
                response.ErrorMessage = "Provide a known stock market ticker!";
                Response.StatusCode = (int)HttpStatusCode.NotFound;

                return response;
            }

            try
            {
                var result = await _dbContext
                .LoadStoredProcedure("dbo.spGetDailyStockSummarySingleCompany")
                .WithSqlParams(
                (nameof(fromDate), fromDate),
                (nameof(ticker), ticker))
                .ExecuteStoredProcedureAsync<StocksCurrentDaySummary>();

                response.Response = result.FirstOrDefault();
            }

            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        [HttpGet("dailyData/{ticker}")]
        public async Task<ApiResponse<AllStocksHistoricalPricesDaily>>
            GetTickerDailyData([NotNull] string ticker)
        {
            var startDate = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);

            var response = new ApiResponse<AllStocksHistoricalPricesDaily>();

            var result = new AllStocksHistoricalPricesDaily();

            try
            {
                var ohlcPrices = await _dbContext
                    .LoadStoredProcedure("dbo.spGetPeriodicalSummary")
                    .WithSqlParams(
                    (nameof(startDate), startDate),
                    (nameof(ticker), ticker))
                    .ExecuteStoredProcedureAsync<OHLCPriceValue>();

                var companyData = await _dbContext
                    .Companies.Where(e => e.Ticker == ticker)
                    .FirstOrDefaultAsync();

                result.HistoricalPrices = ohlcPrices;
                result.Name = companyData.Name;
                result.Ticker = companyData.Ticker;
                result.UrlLogo = companyData.UrlLogo;
                result.Description = companyData.Description;

                response.Response = result;
            }

            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        

        [HttpGet("historicalData/{ticker}")]
        public ApiResponse<AllStocksPriceHistoryModel> GetAllMinutelyHistoricalData
                                                            ([NotNull] string ticker)
        {
            var result = new AllStocksPriceHistoryModel();

            var response = new ApiResponse<AllStocksPriceHistoryModel>();

            if (!Enum.IsDefined(typeof(StocksTicker), ticker.ToUpper()))
            {
                response.ErrorMessage = "Provide a known stock market ticker!";
                Response.StatusCode = (int)HttpStatusCode.NotFound;

                return response;
            }

            var historicalData = new List<TimestampPrices>();

            var companyInfo = default(Company);

            try
            {
                historicalData = _dbContext.PricesData
                .Where(e => e.CompanyTicker == ticker && e.Prediction == false)
                .Select(e => new TimestampPrices()
                {
                    Prediction = false,
                    Price = e.Price,
                    Date = e.Date
                })
                .OrderBy(e => e.Date)
                .AsNoTracking()
                .ToList();

                _dbContext.EnsureCompaniesDataExists();

                companyInfo = _dbContext.Companies
                    .Where(e => e.Ticker == ticker)
                    .FirstOrDefault();
            }

            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                return response;
            }

            result.Ticker = companyInfo.Ticker;
            result.Name = companyInfo.Name;
            result.Description = companyInfo.Description;
            result.UrlLogo = companyInfo.UrlLogo;
            result.HistoricalPrices = historicalData;

            response.Response = result;

            return response;
        }

        [HttpGet("forecastData/{ticker}")]
        public ApiResponse<AllStocksPricePredictionsModel> GetMinutelyForecasts
                                                        ([NotNull] string ticker)
        {
            var result = new AllStocksPricePredictionsModel();

            var response = new ApiResponse<AllStocksPricePredictionsModel>();

            if (!Enum.IsDefined(typeof(StocksTicker), ticker.ToUpper()))
            {
                response.ErrorMessage = "Provide a known stock market ticker!";
                Response.StatusCode = (int)HttpStatusCode.NotFound;

                return response;
            }

            var forecastData = new List<TimestampPrices>();

            var companyInfo = default(Company);

            try
            {
                forecastData = _dbContext.PricesData
                .Where(e => e.CompanyTicker == ticker && e.Prediction == true)
                .Select(e => new TimestampPrices()
                {
                    Prediction = true,
                    Price = e.Price,
                    Date = e.Date
                })
                .OrderBy(e => e.Date)
                .AsNoTracking()
                .ToList();

                _dbContext.EnsureCompaniesDataExists();

                companyInfo = _dbContext.Companies
                    .Where(e => e.Ticker == ticker)
                    .FirstOrDefault();
            }

            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }

            result.Ticker = companyInfo.Ticker;
            result.Name = companyInfo.Name;
            result.Description = companyInfo.Description;
            result.UrlLogo = companyInfo.UrlLogo;
            result.Predictions = forecastData;

            response.Response = result;

            return response;
        }

    }
}
