using Microsoft.AspNetCore.Mvc;
using StocksProccesing.Relational.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using Stocks.General.ExtensionMethods;
using StocksProcessing.API.Models;
using StocksProccesing.Relational.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

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
        public IEnumerable<StocksDailySummaryModel> Get()
        {
            var currentDay = DateTimeOffset.UtcNow.AddDays(-5);

            currentDay -= currentDay.TimeOfDay;

            var result = _dbContext.Set<StocksDailySummaryModel>()
                .FromSqlRaw("exec dbo.spGetDailyStockSummary {0}", currentDay)
                .AsNoTracking();

            return result;

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
        [HttpGet("report2")]
        public List<StocksPriceData> Get2()
        {
            var list = _dbContext.PricesData.Where(e => e.CompanyTicker == "TSLA").AsNoTracking();

            return list.ToList();
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
