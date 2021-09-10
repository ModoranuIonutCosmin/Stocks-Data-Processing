using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Model;
using StocksProcessing.API.Models;
using StocksProcessing.API.Payloads;
using System;
using Stocks.General.ExtensionMethods;
using Stocks.General.ConstantsConfig;
using System.Threading.Tasks;
using System.Linq;
using StocksProcessing.API.Auth;

// For more information on enabling Web API for empty
// projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StocksProcessing.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PortofolioController : ControllerBase
    {
        private readonly StocksMarketContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public PortofolioController(StocksMarketContext context,
            UserManager<ApplicationUser> userManager)
        {
            _dbContext = context;
            _userManager = userManager;
        }

        [HttpGet("buyOrder")]
        [AuthorizeToken]
        public async Task<ApiResponse<OrderTaxesPreview>> GetTransactionTaxesBuy(string ticker, double invested, double leverage)
        {
            var currentDate = DateTimeOffset.UtcNow.SetTime(8, 0);
            var response = new ApiResponse<OrderTaxesPreview>();
            var result = new OrderTaxesPreview();

            var userRequsting = await _userManager.GetUserAsync(HttpContext.User);

            try
            {
                var todaysPrices = _dbContext.PricesData
                                .Where(e => e.Date >= currentDate && e.CompanyTicker == ticker.ToUpper())
                                .OrderBy(e => e.Date)
                                .ToList();
                var openPrice = todaysPrices.FirstOrDefault().Price;
                var spreadTotalFees = TaxesConfig.SpreadDefault; //buy price (sell price + spread)

                if(leverage > 1)
                {
                    spreadTotalFees = TaxesConfig.SpreadFee; // sell + (spread fee)
                }

                result.CurrentPrice = todaysPrices.LastOrDefault().Price; //sell price
                result.CurrentPrice += result.CurrentPrice * spreadTotalFees; //buy price (sell price + spread)

                result.Trend = (result.CurrentPrice / openPrice) - 1;

                result.TodayIncrement = result.CurrentPrice - openPrice;
            }
            catch(Exception ex)
            {
                response.ErrorMessage = ex.Message;

                return response;
            }

            if(invested > userRequsting.Capital || invested == 0)
            {
                response.ErrorMessage = "You can't afford to place this investment!";

                return response;
            }
            
            result.UnitsPaid = invested / result.CurrentPrice;
            result.PercentageExposed = (leverage * invested) / userRequsting.Capital;
            result.WeekdayTax = (TaxesConfig.BuyInterestRate + BankExchangeConsts.LiborOneMonthRatio) / 365 * invested;
            result.WeekendTax = TaxesConfig.WeekendRatioOvernightTax * result.WeekdayTax;

            response.Response = result;

            return response;
        }
    }
}
