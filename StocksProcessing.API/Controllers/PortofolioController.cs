﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stocks.General.Models;
using Stocks.General.ExtensionMethods;
using StocksProccesing.Relational.Model;
using StocksProcessing.API.Auth;
using StocksProcessing.API.Payloads;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Stocks.General.ConstantsConfig;
using StocksProccesing.Relational.Repositories;
using StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator;

// For more information on enabling Web API for empty
// projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StocksProcessing.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [AuthorizeToken]
    public class PortofolioController : ControllerBase
    {
        private readonly IStockMarketDisplayPriceCalculator _priceCalculator;
        private readonly IStockMarketOrderTaxesCalculator _taxesCalculator;
        private readonly IStockPricesRepository pricesRepository;
        private readonly IUsersRepository usersRepository;
        private readonly ITransactionsRepository transactionsRepository;
        private readonly IOrdersRepository ordersRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public PortofolioController(
            IStockMarketDisplayPriceCalculator priceCalculator,
            IStockMarketOrderTaxesCalculator taxesCalculator,
            IStockPricesRepository pricesRepository,
            IUsersRepository usersRepository,
            ITransactionsRepository transactionsRepository,
            IOrdersRepository ordersRepository,
            UserManager<ApplicationUser> userManager)
        {
            _priceCalculator = priceCalculator;
            _taxesCalculator = taxesCalculator;
            this.pricesRepository = pricesRepository;
            this.usersRepository = usersRepository;
            this.transactionsRepository = transactionsRepository;
            this.ordersRepository = ordersRepository;
            _userManager = userManager;
        }

        [HttpPost("refillBalance")]
        public async Task<ApiResponse<BalanceRefillOrder>> ReplenishBalance([FromBody] PaymentDetails paymentDetails)
        {
            var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

            if (userRequesting is null)
                return this.FailedApiOperationResponse<BalanceRefillOrder>
                    ("User is not authorized", statusCode: HttpStatusCode.Unauthorized);

            var order = new Order()
            {
                Amount = paymentDetails.Amount,
                CurrencyTicker = paymentDetails.InitialCurrencyTicker,
                DateFinalized = paymentDetails.PaymentDate,
                PaymentProcessor = paymentDetails.PaymentHandler,
            };

            await ordersRepository.PlaceRefillBalanceOrder(userRequesting, order);

            return new ApiResponse<BalanceRefillOrder>()
            {
                Response = new BalanceRefillOrder()
                {
                    AmountBought = paymentDetails.Amount,
                    CurrentBalance = userRequesting.Capital
                }
            };
        }

        [HttpGet("previewPlaceOrder")]
        public async Task<ApiResponse<OrderTaxesPreview>> GetTransactionTaxes(string ticker, decimal invested,
            decimal leverage, bool isBuy)
        {
            var response = new ApiResponse<OrderTaxesPreview>();
            var result = new OrderTaxesPreview();

            var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

            if (userRequesting is null)
                return this.FailedApiOperationResponse<OrderTaxesPreview>(reason: "User is unauthorized!",
                    statusCode: HttpStatusCode.Unauthorized);

            if (invested > userRequesting.Capital || invested == 0)
            {
                result.ExtraMoneyNeeded = Math.Abs(invested - userRequesting.Capital);

                return this.FailedApiOperationResponse(reason: "You can't afford to place this investment!",
                    value: result);
            }

            var todaysPrices = pricesRepository.GetTodaysPriceEvolution(ticker);

            if(todaysPrices.Count == 0)
                return this.FailedApiOperationResponse<OrderTaxesPreview>
                    (reason: "An error occured. No records for current day!");

            var openPrice = todaysPrices.First().Price;

            var currentStocksPriceWithoutFees = todaysPrices.Last().Price; // sell price

            if (isBuy)
                result.CurrentPrice = _priceCalculator.CalculateBuyPrice(currentStocksPriceWithoutFees, leverage);
            else
                result.CurrentPrice = _priceCalculator.CalculateSellPrice(currentStocksPriceWithoutFees, leverage);

            result.TodayIncrement = (currentStocksPriceWithoutFees - openPrice).TruncateToDecimalPlaces(3);
            result.Trend = ((result.CurrentPrice / openPrice) - 1).TruncateToDecimalPlaces(3);

            var effectiveMoney = leverage * invested;
            var borrowedMoney = effectiveMoney - invested;

            result.InvestedAmount = invested;
            result.UnitsPaid = (effectiveMoney / result.CurrentPrice).TruncateToDecimalPlaces(3);
            result.PercentageExposed = (effectiveMoney / userRequesting.Capital).TruncateToDecimalPlaces(3);
            result.WeekdayTax = _taxesCalculator.CalculateWeekDayTax(leverage, borrowedMoney, isBuy);
            result.WeekendTax = _taxesCalculator.CalculateWeekEndTax(leverage, borrowedMoney, isBuy);

            response.Response = result;

            return response;
        }


        [HttpPost("placeOrder")]
        public async Task<ApiResponse<MarketOrder>> PlaceOrder([FromBody] MarketOrder marketOrder)
        {

            var response = new ApiResponse<MarketOrder>();
            var result = new MarketOrder();

            if (!DateTimeOffset.UtcNow.IsDayTimeBetweenStockTradingRange())
                return this.FailedApiOperationResponse<MarketOrder>(reason: "Stock market is closed right now!");

            var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

            if (userRequesting is null)
                return this.FailedApiOperationResponse<MarketOrder>(reason: "User is unauthorized!",
                           statusCode: HttpStatusCode.Unauthorized);

            if (transactionsRepository.ExistsTransaction(marketOrder.Token))
                return this.FailedApiOperationResponse<MarketOrder>
                        (reason: "An error occured. You've already placed this order!");

            if (marketOrder.InvestedAmount > userRequesting.Capital ||
                marketOrder.InvestedAmount == 0
                )
                return this.FailedApiOperationResponse<MarketOrder>(
                    reason: "You can't afford to place this investment!");

            if (!TaxesConfig.Leverages.Contains(marketOrder.Leverage))
                return this.FailedApiOperationResponse<MarketOrder>(
                    reason: "Invalid leverage value!");

            if (marketOrder.Leverage > 1 &&
                marketOrder.StopLossAmount > marketOrder.InvestedAmount * TaxesConfig.StopLossMaxPercent ||
                marketOrder.StopLossAmount < 0 || marketOrder.TakeProfitAmount < 0)
                return this.FailedApiOperationResponse<MarketOrder>(
                   reason: "Either stop loss or take profit can't have such values!");

            var todaysPrices = pricesRepository.GetTodaysPriceEvolution(marketOrder.Ticker);

            if (todaysPrices.Count == 0)
                return this.FailedApiOperationResponse<MarketOrder>
                    (reason: "An error occured. No records for current day!");

            var currentPrice = pricesRepository.GetCurrentUnitPriceByStocksCompanyTicker(marketOrder.Ticker);
            var sellPrice = _priceCalculator.CalculateSellPrice(currentPrice, marketOrder.Leverage);
            var buyPrice = _priceCalculator.CalculateBuyPrice(currentPrice, marketOrder.Leverage);

            var transaction = new StocksTransaction()
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

            if (!(await usersRepository.OpenUserTransaction(userRequesting, transaction)))
                return this.FailedApiOperationResponse<MarketOrder>
                    (reason: $"Can't place the order. Try again later. OpenUserTransaction ->");

            response.Response = marketOrder;

            return response;
        }

        [HttpGet("tradingContext")]
        public async Task<ApiResponse<TradingContext>> GetTradingContext()
        {
            var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

            if (userRequesting is null)
                return this.FailedApiOperationResponse<TradingContext>(reason: "User is unauthorized!",
                       statusCode: HttpStatusCode.Unauthorized);

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
