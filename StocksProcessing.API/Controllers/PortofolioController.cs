﻿using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Stocks.General.ExtensionMethods;
using Stocks.General.ConstantsConfig;
using StocksProcessing.API.Auth;
using StocksProcessing.API.Models;
using StocksProcessing.API.Payloads;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Model;
using StocksProccesing.Relational.Helpers;
using System.Net;

// For more information on enabling Web API for empty
// projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StocksProcessing.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [AuthorizeToken]
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
        public async Task<ApiResponse<OrderTaxesPreview>> GetTransactionTaxesBuy(string ticker, double invested, double leverage)
        {
            var currentDate = DateTimeOffset.UtcNow.AddDays(-20).SetTime(8, 0);
            var response = new ApiResponse<OrderTaxesPreview>();
            var result = new OrderTaxesPreview();

            var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

            if (userRequesting is null)
            {
                response.ErrorMessage = "User is unauthorized!";

                Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                return response;
            }

            if (invested > userRequesting.Capital || invested == 0)
            {
                response.ErrorMessage = "You can't afford to place this investment!";

                response.Response = result;

                result.ExtraMoneyNeeded = Math.Abs(invested - userRequesting.Capital);

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


            result.TodayIncrement = (currentStocksPriceWithoutFees - openPrice).TruncateToDecimalPlaces(3);


            var spreadTotalFees = leverage > 1 ? TaxesConfig.SpreadFee // buy price + (spread fee)
                                               : TaxesConfig.AverageStockMarketSpread; //buy price (sell price + spread)

            result.CurrentPrice += currentStocksPriceWithoutFees
                + currentStocksPriceWithoutFees * spreadTotalFees; //buy price (sell price + spread)

            result.CurrentPrice = result.CurrentPrice.TruncateToDecimalPlaces(3);

            result.Trend = ((result.CurrentPrice / openPrice) - 1).TruncateToDecimalPlaces(3);
            var effectiveMoney = leverage * invested;

            result.InvestedAmount = invested;
            result.UnitsPaid = (effectiveMoney / result.CurrentPrice).TruncateToDecimalPlaces(3);
            result.PercentageExposed = (effectiveMoney / userRequesting.Capital).TruncateToDecimalPlaces(3);

            if (leverage == 1)
                result.WeekdayTax = 0;
            else
                result.WeekdayTax = ((TaxesConfig.BuyInterestRate
                    + BankExchangeConsts.LiborOneMonthRatio) / 365 * effectiveMoney).TruncateToDecimalPlaces(3);

            response.Response = result;

            return response;
        }


        [HttpPost("placeOrder")]
        public async Task<ApiResponse<MarketOrder>> PlaceOrder([FromBody] MarketOrder marketOrder)
        {
            //var lastMarketDate = DateTimeOffset.UtcNow.GetClosestPreviousStockMarketDateTime();
            var lastMarketDate = DateTimeOffset.UtcNow.AddDays(-20).SetTime(8, 0);

            var response = new ApiResponse<MarketOrder>();
            var result = new MarketOrder();

            if (!DateTimeOffset.UtcNow.IsDayTimeBetweenStockTradingRange())
            {
                response.ErrorMessage = "Stock market is closed right now!";

                return response;
            }

            var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

            if (userRequesting is null)
            {
                response.ErrorMessage = "User is unauthorized!";

                Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                return response;
            }

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

            if (marketOrder.Leverage > 1)
            {
                if (marketOrder.StopLossAmount > marketOrder.InvestedAmount * TaxesConfig.StopLossMaxPercent
                    || marketOrder.StopLossAmount < 0 || marketOrder.TakeProfitAmount < 0)
                {
                    response.ErrorMessage = "Either stop loss or take profit can't have such values!";

                    return response;
                }
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
                    .Where(k => k.Date >= lastMarketDate && k.CompanyTicker == marketOrder.Ticker.ToUpper())
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
            var buyPrice = StockMarketCalculus.CalculateBuyPrice(sellPrice);

            var transaction = new PortofolioOpenTransaction()
            {
                StopLossAmount = marketOrder.StopLossAmount,
                TakeProfitAmount = marketOrder.TakeProfitAmount,
                Date = DateTimeOffset.UtcNow,
                InvestedAmount = marketOrder.InvestedAmount,
                UniqueActionStamp = marketOrder.Token,
                IsBuy = marketOrder.IsBuy,
                Open = true,
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
            catch (Exception ex)
            {
                response.ErrorMessage = $"Can't place the order. Try again later. | {ex.Message}";

                return response;
            }
            response.Response = marketOrder;

            return response;
        }





        [HttpGet("sellOrder")]
        public async Task<ApiResponse<OrderTaxesPreview>> GetTransactionTaxesSell(string ticker, double invested, double leverage)
        {
            var currentDate = DateTimeOffset.UtcNow.AddDays(-20).SetTime(8, 0);
            //var currentDate = DateTimeOffset.UtcNow.GetClosestPreviousStockMarketDateTime();

            var response = new ApiResponse<OrderTaxesPreview>();
            var result = new OrderTaxesPreview
            {
                InvestedAmount = invested
            };

            var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

            if (userRequesting is null)
            {
                response.ErrorMessage = "User is unauthorized!";

                Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                return response;
            }

            if (invested > userRequesting.Capital || invested == 0)
            {
                response.ErrorMessage = "You can't afford to place this investment!";

                response.Response = result;

                result.ExtraMoneyNeeded = Math.Abs(invested - userRequesting.Capital);

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
            var effectiveMoney = leverage * invested;

            result.CurrentPrice = (todaysPrices.Last().Price).TruncateToDecimalPlaces(3); //sell price
            result.TodayIncrement = (result.CurrentPrice - openPrice).TruncateToDecimalPlaces(3);
            result.Trend = (((result.CurrentPrice / openPrice) - 1) * 100).TruncateToDecimalPlaces(3);
            result.UnitsPaid = (effectiveMoney / result.CurrentPrice).TruncateToDecimalPlaces(3);
            result.PercentageExposed = (effectiveMoney / userRequesting.Capital).TruncateToDecimalPlaces(3);

            result.WeekdayTax = ((TaxesConfig.SellInterestRate
                + BankExchangeConsts.LiborOneMonthRatio) / 365 * effectiveMoney).TruncateToDecimalPlaces(3);

            response.Response = result;

            return response;
        }



        [HttpGet("openTransactions")]
        public async Task<ApiResponse<AllTransactionsGroupedSummary>> GatherTransactionsSummary()
        {
            var response = new ApiResponse<AllTransactionsGroupedSummary>();

            var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

            if (userRequesting is null)
            {
                response.ErrorMessage = "User is unauthorized!";

                Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                return response;
            }

            var transactionsSummary = new List<CompanyTransactionsSummary>();
            var openTransactionsList = new List<TransactionSummary>();

            try
            {
                var companiesData = _dbContext.Companies.ToList();

                openTransactionsList = _dbContext.Transactions
                    .Where(e => e.ApplicationUserId == userRequesting.Id && e.Open == true)
                    .Select(e =>
                    new TransactionSummary()
                    {
                        CurrentSellPrice = _dbContext.GatherCurrentPriceByCompany(e.Ticker),
                        InitialPrice = e.IsBuy ? e.UnitBuyPriceThen : e.UnitSellPriceThen,
                        InvestedAmount = e.InvestedAmount,
                        IsBuy = e.IsBuy,
                        Ticker = e.Ticker,
                        UnitsPurchased = e.InvestedAmount / (e.IsBuy ? e.UnitBuyPriceThen : e.UnitSellPriceThen)
                    }).AsNoTracking().ToList();

                transactionsSummary = openTransactionsList
                    .GroupBy(c => c.Ticker)
                    .Select(g => new CompanyTransactionsSummary()
                    {
                        Ticker = g.Key,
                        AverageInitial = (g.Average(e => e.InitialPrice)).TruncateToDecimalPlaces(3),
                        TotalInvested = (g.Sum(e => e.InvestedAmount)).TruncateToDecimalPlaces(3),
                        TotalPl = (g.Sum(e => e.ProfitOrLoss)).TruncateToDecimalPlaces(3),
                        TotalPlPercentage = (g.Sum(e => e.ProfitOrLoss) / g.Sum(e => e.InvestedAmount) * 100).TruncateToDecimalPlaces(3),
                        TotalUnits = (g.Sum(e => e.UnitsPurchased)).TruncateToDecimalPlaces(3),
                        Value = (g.Sum(e => e.UnitsPurchased * e.InitialPrice + e.ProfitOrLoss)).TruncateToDecimalPlaces(3)
                    })
                    .Join(companiesData, e => e.Ticker, e => e.Ticker,
                    (transaction, company) =>
                    {
                        transaction.UrlLogo = company.UrlLogo;
                        transaction.Name = company.Name;
                        transaction.Description = company.Description;

                        return transaction;
                    })
                    .ToList();
            }
            catch (Exception e)
            {
                response.ErrorMessage = $"Couldn't get opened transactions data! | {e.Message}";

                return response;
            }

            response.Response = new AllTransactionsGroupedSummary
            {
                Transactions = transactionsSummary
            };

            return response;
        }

        [HttpGet("openTransactionsForTicker/{ticker}")]
        public async Task<ApiResponse<AllTransactionsDetailed>> GatherTransactionsParticularTicker(string ticker)
        {
            var response = new ApiResponse<AllTransactionsDetailed>();

            var result = new AllTransactionsDetailed();

            var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

            if (userRequesting is null)
            {
                response.ErrorMessage = "User is unauthorized!";

                Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                return response;
            }

            var openTransactionsList = new List<TransactionFullInfo>();

            try
            {
                var companyData = _dbContext.Companies.First(e => e.Ticker == ticker);

                openTransactionsList = _dbContext.Transactions
                    .Where(e => e.ApplicationUserId == userRequesting.Id
                    && e.Ticker == ticker.ToUpper() && e.Open == true)
                    .AsNoTracking()
                    .AsEnumerable()
                    .Select(e =>
                    {
                        var currentSellPrice = _dbContext.GatherCurrentPriceByCompany(e.Ticker);

                        var currentPrice = e.IsBuy ? currentSellPrice : StockMarketCalculus.CalculateBuyPrice(currentSellPrice);

                        return new TransactionFullInfo()
                        {
                            Id = e.Id,
                            CurrentPrice = (currentPrice).TruncateToDecimalPlaces(3),
                            InitialPrice = (e.IsBuy ? e.UnitBuyPriceThen : e.UnitSellPriceThen).TruncateToDecimalPlaces(3),
                            IsBuy = e.IsBuy,
                            Leverage = e.Leverage,
                            InvestedAmount = e.InvestedAmount,
                            UnitsPurchased = (e.InvestedAmount / (e.IsBuy ? e.UnitBuyPriceThen : e.UnitSellPriceThen)).TruncateToDecimalPlaces(3),
                            StopLossAmount = e.StopLossAmount.TruncateToDecimalPlaces(3),
                            TakeProfitAmount = e.TakeProfitAmount.TruncateToDecimalPlaces(3),
                            Date = e.Date
                        };
                    }).ToList();

                result.UrlLogo = companyData.UrlLogo;
                result.Name = companyData.Name;
                result.Description = companyData.Description;
                result.Ticker = ticker;
            }
            catch (Exception e)
            {
                response.ErrorMessage = $"Couldn't get opened transactions data! | {e.Message}";

                return response;
            }

            result.Transactions = openTransactionsList;

            response.Response = result;

            return response;
        }

        [HttpPost("closeTransaction")]
        public async Task<ApiResponse<CloseTransactionRequest>> CancelTransaction([FromBody]CloseTransactionRequest request)
        {
            var response = new ApiResponse<CloseTransactionRequest>();

            try
            {
                var transaction = _dbContext.Transactions
                .First(e => e.Id == request.Id);

                var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

                if (userRequesting is null || transaction is null || transaction.ApplicationUserId != userRequesting.Id)
                {

                    response.ErrorMessage = "User is unauthorized!";

                    Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                    return response;
                }

                var currentSellPrice = _dbContext.GatherCurrentPriceByCompany(transaction.Ticker);

                var currentBuyPrice = StockMarketCalculus.CalculateBuyPrice(currentSellPrice);

                var initialSellPrice = transaction.UnitSellPriceThen;
                var initialBuyPrice = transaction.UnitBuyPriceThen;

                var profitOrLoss = default(double);

                if (transaction.IsBuy)
                {
                    profitOrLoss = (currentSellPrice - initialBuyPrice) * (transaction.InvestedAmount / initialBuyPrice);
                } 
                else
                {
                    profitOrLoss = (initialSellPrice - currentBuyPrice) * (transaction.InvestedAmount / initialSellPrice);
                }

                userRequesting.Capital += transaction.InvestedAmount + profitOrLoss;
                transaction.Open = false;

                await _dbContext.SaveChangesAsync();
            }

            catch(Exception ex)
            {
                response.ErrorMessage = $"An error occured. Please try again later. | {ex.Message}";

                return response;
            }

            response.Response = request;

            return response;
        }

        [HttpGet("tradingContext")]
        public async Task<ApiResponse<TradingContext>> GetTradingContext()
        {
            var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

            if (userRequesting is null)
            {
                var response = new ApiResponse<TradingContext>();

                response.ErrorMessage = "User is unauthorized!";

                Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                return response;
            }

            return new ApiResponse<TradingContext>()
            {
                Response = new TradingContext()
                {
                    Funds = userRequesting.Capital
                }
            };
        }
    }
}
