using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StocksProccesing.Relational.DataAccess;
using StocksProcessing.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using StocksProccesing.Relational.Extension_Methods;
using Stocks.General.ExtensionMethods;
using StocksProcessing.API.Payloads;
using StocksProccesing.Relational.Model;
using Stocks.General;
using System.Diagnostics.CodeAnalysis;
using System.Net;

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

        [HttpGet("report")]
        public ApiResponse<List<StocksDailySummaryModel>> GetReportsAllCompanies()
        {
            var currentDayStart = DateTimeOffset.UtcNow.AddDays(-6).SetTime(8, 0);

            var response = new ApiResponse<List<StocksDailySummaryModel>>();

            try
            {
                var result = _dbContext.Set<StocksDailySummaryModel>()
                .FromSqlRaw("exec dbo.spGetDailyStockSummary {0}", currentDayStart)
                .AsNoTracking();

                response.Response = result.ToList();
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
        public ApiResponse<StocksDailySummaryModel> GetReportsByCompany([NotNull] string ticker)
        {
            //TODO: Remove this
            var currentDayStart = DateTimeOffset.UtcNow.AddDays(-6).SetTime(8, 0);

            var response = new ApiResponse<StocksDailySummaryModel>();

            if (!Enum.IsDefined(typeof(StocksTicker), ticker))
            {
                response.ErrorMessage = "Provide a known stock market ticker!";
                Response.StatusCode = (int)HttpStatusCode.NotFound;

                return response;
            }

            try
            {
                var result = _dbContext.Set<StocksDailySummaryModel>()
                .FromSqlRaw("exec dbo.spGetDailyStockSummarySingleCompany {0}, {1}",
                new object[] { currentDayStart, ticker })
                .AsNoTracking().AsEnumerable();

                response.Response = result.FirstOrDefault();
            }

            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        [HttpGet("historicalData/{ticker}")]
        public ApiResponse<WholeStocksPriceHistoryModel> GetAllMinutelyHistoricalData
                                                            ([NotNull] string ticker)
        {
            var result = new WholeStocksPriceHistoryModel();

            var response = new ApiResponse<WholeStocksPriceHistoryModel>();

            if (!Enum.IsDefined(typeof(StocksTicker), ticker))
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
                .Where(e => e.CompanyTicker == ticker)
                .OrderBy(c => c.Date)
                .Select(e => new TimestampPrices()
                {
                    Prediction = e.Prediction,
                    Price = e.Price,
                    TimeStamp = e.Date
                })
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
            result.HistoricalPrices = historicalData;

            response.Response = result;

            return response;
        }

        [HttpGet("forecastData/{ticker}")]
        public ApiResponse<WholeStocksPricePredictionsModel> GetMinutelyForecasts
                                                        ([NotNull] string ticker)
        {
            var result = new WholeStocksPricePredictionsModel();

            var response = new ApiResponse<WholeStocksPricePredictionsModel>();

            if (!Enum.IsDefined(typeof(StocksTicker), ticker))
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
                .OrderBy(c => c.Date)
                .Select(e => new TimestampPrices()
                {
                    Prediction = true,
                    Price = e.Price,
                    TimeStamp = e.Date
                })
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
