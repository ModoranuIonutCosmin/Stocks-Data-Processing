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
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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

        [AuthorizeToken]
        [HttpPost("refillBalance")]
        public async Task<ApiResponse<BalanceRefillOrder>> ReplenishBalance([FromBody] PaymentDetails paymentDetails)
        {
            var userRequsting = await _userManager.GetUserAsync(HttpContext.User);

            userRequsting.Capital += paymentDetails.Amount;

            await _dbContext.Orders.AddAsync(new Order()
            {
                Amount = paymentDetails.Amount,
                CurrencyTicker = paymentDetails.InitialCurrencyTicker,
                DateFinalized = paymentDetails.PaymentDate,
                PaymentProcessor = paymentDetails.PaymentHandler,
            });

            await _dbContext.SaveChangesAsync();

            return new ApiResponse<BalanceRefillOrder>()
            {
                Response = new BalanceRefillOrder()
                {
                    AmountBought = paymentDetails.Amount,
                    CurrentBalance = userRequsting.Capital
                }
            };
        }

        [HttpGet("buyOrder")]
        [AuthorizeToken]
        public async Task<ApiResponse<OrderTaxesPreview>> GetTransactionTaxesBuy(string ticker, double invested, double leverage)
        {
            var currentDate = DateTimeOffset.UtcNow.AddDays(-20).SetTime(8, 0);
            var response = new ApiResponse<OrderTaxesPreview>();
            var result = new OrderTaxesPreview();

            var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

            if (invested > userRequesting.Capital || invested == 0)
            {
                response.ErrorMessage = "You can't afford to place this investment!";

                response.Response = result;

                result.ExtraMoneyNeeded = invested - userRequesting.Capital;

                return response;
            }

            var todaysPrices = new List<StocksPriceData>();

            try
            {
                todaysPrices = _dbContext.PricesData
                                .Where(e => e.Date >= currentDate && e.CompanyTicker == ticker.ToUpper())
                                .OrderBy(e => e.Date)
                                .AsNoTracking()
                                .ToList();


                if (todaysPrices.Count == 0)
                {
                    throw new Exception("No stocks price data available!");
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;

                return response;
            }

            var openPrice = todaysPrices.First().Price;


            var currentStocksPriceWithoutFees = todaysPrices.Last().Price; // sell price

            result.Trend = Math.Round(((result.CurrentPrice / openPrice) - 1) * 100, 3);

            result.TodayIncrement = Math.Round(currentStocksPriceWithoutFees - openPrice, 3);


            var spreadTotalFees = leverage > 1 ? TaxesConfig.SpreadFee // buy price + (spread fee)
                                               : TaxesConfig.AverageStockMarketSpread; //buy price (sell price + spread)

            result.CurrentPrice += currentStocksPriceWithoutFees
                + Math.Round(currentStocksPriceWithoutFees * spreadTotalFees, 3); //buy price (sell price + spread)

            var effectiveMoney = leverage * invested;
            var borrowedMoney = effectiveMoney - invested;

            result.UnitsPaid = Math.Round(effectiveMoney / result.CurrentPrice, 3);
            result.PercentageExposed = Math.Round(effectiveMoney / userRequesting.Capital, 3);

            if (leverage == 1)
                result.WeekdayTax = 0;
            else
                result.WeekdayTax = Math.Round((TaxesConfig.BuyInterestRate
                    + BankExchangeConsts.LiborOneMonthRatio) / 365 * borrowedMoney, 3);

            response.Response = result;

            return response;
        }

        [HttpPost("buyOrder")]
        [AuthorizeToken]
        public async Task<ApiResponse<MarketOrder>> PlaceBuyOrder([FromBody] MarketOrder marketOrder)
        {
            var lastMarketDate = DateTimeOffset.UtcNow.GetClosestPreviousStockMarketDateTime();
            var response = new ApiResponse<MarketOrder>();
            var result = new MarketOrder();

            if (!DateTimeOffset.UtcNow.IsDayTimeBetweenStockTradingRange())
            {
                response.ErrorMessage = "Stock market is closed right now!";

                return response;
            }

            var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

            if (marketOrder.InvestedAmount > userRequesting.Capital ||
                marketOrder.InvestedAmount == 0
                )
            {
                response.ErrorMessage = "You can't afford to place this investment!";

                return response;
            }

            if (!TaxesConfig.Leverages.Contains(marketOrder.Leverage))
            {
                response.ErrorMessage = "Invalid leverage value!";

                return response;
            }


            var todaysPrices = new List<StocksPriceData>();

            try
            {
                if (_dbContext.Transactions
                    .Any(e => e.UniqueActionStamp == marketOrder.Token))
                {
                    throw new Exception("You've already placed this order");
                }

                todaysPrices = _dbContext.PricesData
                    .Where(k => k.Date >= lastMarketDate)
                    .OrderBy(e => e.Date)
                    .AsNoTracking()
                    .ToList();
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;

                return response;
            }

            if (todaysPrices.Count == 0)
            {
                response.ErrorMessage = "Can't place the order. Not enough stocks data!";

                return response;
            }

            var sellPrice = todaysPrices.Last().Price;
            var buyPrice = default(double);

            if (marketOrder.Leverage > 1)
            {
                buyPrice += sellPrice * (TaxesConfig.AverageStockMarketSpread + TaxesConfig.SpreadFee);


                if (marketOrder.StopLossAmount >= marketOrder.InvestedAmount * TaxesConfig.StopLossMaxPercent)
                {
                    response.ErrorMessage = "Stop loss can't be that permissive!";

                    return response;
                }
            }
            else
            {
                buyPrice += sellPrice * TaxesConfig.AverageStockMarketSpread;
            }

            var transaction = new PortofolioOpenTransaction()
            {
                StopLossAmount = marketOrder.StopLossAmount,
                TakeProfitAmount = marketOrder.TakeProfitAmount,
                Date = DateTimeOffset.UtcNow,
                InvestedSum = marketOrder.InvestedAmount,
                UniqueActionStamp = marketOrder.Token,
                IsBuy = true,
                UnitSellPriceThen = sellPrice,
                UnitBuyPriceThen = buyPrice,
                Leverage = marketOrder.Leverage,
                Ticker = marketOrder.Ticker,
                ApplicationUserId = userRequesting.Id
            };

            try
            {
                _dbContext.Transactions.Add(transaction);
                userRequesting.Capital -= marketOrder.InvestedAmount;

                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                response.ErrorMessage = "Can't place the order. Try again later.";

                return response;
            }
            response.Response = marketOrder;

            return response;
        }


        [HttpPost("sellOrder")]
        [AuthorizeToken]
        public async Task<ApiResponse<MarketOrder>> PlaceSellOrder([FromBody] MarketOrder marketOrder)
        {
            var lastMarketDate = DateTimeOffset.UtcNow.GetClosestPreviousStockMarketDateTime();
            var response = new ApiResponse<MarketOrder>();
            var result = new MarketOrder();

            if (!DateTimeOffset.UtcNow.IsDayTimeBetweenStockTradingRange())
            {
                response.ErrorMessage = "Stock market is closed right now!";

                return response;
            }

            var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

            if (marketOrder.InvestedAmount > userRequesting.Capital ||
                marketOrder.InvestedAmount == 0
                )
            {
                response.ErrorMessage = "You can't afford to place this investment!";

                return response;
            }

            if (!TaxesConfig.Leverages.Contains(marketOrder.Leverage))
            {
                response.ErrorMessage = "Invalid leverage value!";

                return response;
            }

            if (marketOrder.StopLossAmount >= marketOrder.InvestedAmount * TaxesConfig.StopLossMaxPercent)
            {
                response.ErrorMessage = "Stop loss can't be that permissive!";

                return response;
            }

            var todaysPrices = new List<StocksPriceData>();

            try
            {
                if (_dbContext.Transactions
                    .Any(e => e.UniqueActionStamp == marketOrder.Token))
                {
                    throw new Exception("You've already placed this order");
                }

                todaysPrices = _dbContext.PricesData
                    .Where(k => k.Date >= lastMarketDate)
                    .OrderBy(e => e.Date)
                    .AsNoTracking()
                    .ToList();
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;

                return response;
            }

            if (todaysPrices.Count == 0)
            {
                response.ErrorMessage = "Can't place the order. Not enough stocks data!";

                return response;
            }

            var sellPrice = todaysPrices.Last().Price;
            var buyPrice = default(double);

            buyPrice += sellPrice * (TaxesConfig.AverageStockMarketSpread + TaxesConfig.SpreadFee);

            var transaction = new PortofolioOpenTransaction()
            {
                StopLossAmount = marketOrder.StopLossAmount,
                TakeProfitAmount = marketOrder.TakeProfitAmount,
                Date = DateTimeOffset.UtcNow,
                InvestedSum = marketOrder.InvestedAmount,
                UniqueActionStamp = marketOrder.Token,
                IsBuy = false,
                UnitSellPriceThen = sellPrice,
                UnitBuyPriceThen = buyPrice,
                Leverage = marketOrder.Leverage,
                Ticker = marketOrder.Ticker,
                ApplicationUserId = userRequesting.Id
            };

            try
            {
                _dbContext.Transactions.Add(transaction);
                userRequesting.Capital -= marketOrder.InvestedAmount;

                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                response.ErrorMessage = "Can't place the order. Try again later.";

                return response;
            }
            response.Response = marketOrder;

            return response;
        }

        [HttpGet("sellOrder")]
        [AuthorizeToken]
        public async Task<ApiResponse<OrderTaxesPreview>> GetTransactionTaxesSell(string ticker, double invested, double leverage)
        {
            var currentDate = DateTimeOffset.UtcNow.AddDays(-20).SetTime(8, 0);
            var response = new ApiResponse<OrderTaxesPreview>();
            var result = new OrderTaxesPreview();

            result.InvestedAmount = invested;

            var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

            if (invested > userRequesting.Capital || invested == 0)
            {
                response.ErrorMessage = "You can't afford to place this investment!";

                response.Response = result;

                result.ExtraMoneyNeeded = invested - userRequesting.Capital;

                return response;
            }

            var todaysPrices = new List<StocksPriceData>();

            try
            {
                todaysPrices = _dbContext.PricesData
                                .Where(e => e.Date >= currentDate && e.CompanyTicker == ticker.ToUpper())
                                .OrderBy(e => e.Date)
                                .AsNoTracking()
                                .ToList();

                if (todaysPrices.Count == 0)
                {
                    throw new Exception("No stocks price data available!");
                }

            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;

                return response;
            }

            var openPrice = todaysPrices.First().Price;

            result.CurrentPrice = Math.Round(todaysPrices.Last().Price); //sell price

            result.TodayIncrement = Math.Round(result.CurrentPrice - openPrice, 3);
            result.Trend = Math.Round(((result.CurrentPrice / openPrice) - 1) * 100, 3);

            var effectiveMoney = leverage * invested;

            result.UnitsPaid = Math.Round(effectiveMoney / result.CurrentPrice, 3);
            result.PercentageExposed = Math.Round(effectiveMoney / userRequesting.Capital, 3);

            var borrowedMoney = effectiveMoney - invested;

            result.WeekdayTax = Math.Round((TaxesConfig.SellInterestRate
                + BankExchangeConsts.LiborOneMonthRatio) / 365 * borrowedMoney, 3);

            response.Response = result;

            return response;
        }
    }




}
