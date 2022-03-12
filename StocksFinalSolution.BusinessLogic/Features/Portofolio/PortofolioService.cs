using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Stocks.General.ConstantsConfig;
using Stocks.General.Exceptions;
using Stocks.General.ExtensionMethods;
using Stocks.General.Models;
using Stocks.General.Models.Funds;
using Stocks.General.Models.StocksInfoAggregates;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Features.Portofolio;

public class PortofolioService : IPortofolioService
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IStockPricesRepository _stockPricesRepository;
    private readonly IStockMarketDisplayPriceCalculator _priceCalculator;
    private readonly IStockMarketOrderTaxesCalculator _taxesCalculator;
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public PortofolioService(IOrdersRepository ordersRepository,
        IStockPricesRepository stockPricesRepository,
        IStockMarketDisplayPriceCalculator priceCalculator,
        IStockMarketOrderTaxesCalculator taxesCalculator,
        ITransactionsRepository transactionsRepository,
        IUsersRepository usersRepository,
        UserManager<ApplicationUser> userManager)
    {
        _ordersRepository = ordersRepository;
        _stockPricesRepository = stockPricesRepository;
        _priceCalculator = priceCalculator;
        _taxesCalculator = taxesCalculator;
        _transactionsRepository = transactionsRepository;
        _usersRepository = usersRepository;
        _userManager = userManager;
    }

    public async Task<BalanceRefillOrder> ReplenishBalance(ApplicationUser userRequesting,
        PaymentDetails paymentDetails)
    {
        var order = new Order()
        {
            Amount = paymentDetails.Amount,
            CurrencyTicker = paymentDetails.InitialCurrencyTicker,
            DateFinalized = paymentDetails.PaymentDate,
            PaymentProcessor = paymentDetails.PaymentHandler,
        };

        await _ordersRepository.PlaceRefillBalanceOrder(userRequesting, order);

        return new BalanceRefillOrder()
        {
            AmountBought = paymentDetails.Amount,
            CurrentBalance = userRequesting.Capital
        };
    }

    public async Task<OrderTaxesPreview> GetTransactionTaxes(ApplicationUser requestingUser,
        string ticker, decimal invested, decimal leverage, bool isBuy)
    {
        if (!DateTimeOffset.UtcNow.IsDateDuringStockMarketOpenTimeframe())
        {
            throw new StockMarketClosedException(
                "Stock market is closed, try again during business hours");
        }

        if (invested > requestingUser.Capital || invested == 0)
        {
            var extraMoneyNeeded = Math.Abs(invested - requestingUser.Capital);

            throw new InsufficientFundsException($"Insufficient funds. Needs {extraMoneyNeeded}");
        }

        var todaysPrices = _stockPricesRepository.GetTodaysPriceEvolution(ticker);

        if (!todaysPrices.Any())
        {
            throw new NoStockPricesRecordedException("Our backend stocks prices store in unavailable." +
                                                     " Try again later");
        }

        var openPrice = todaysPrices.First().Price;
        var currentStocksPriceWithoutFees = todaysPrices.Last().Price; // sell price

        var effectiveMoney = leverage * invested;
        var borrowedMoney = effectiveMoney - invested;

        var currentPrice = isBuy
            ? _priceCalculator.CalculateBuyPrice(currentStocksPriceWithoutFees, leverage)
            : _priceCalculator.CalculateSellPrice(currentStocksPriceWithoutFees, leverage);

        return new OrderTaxesPreview()
        {
            CurrentPrice = currentPrice,
            TodayIncrement = (currentStocksPriceWithoutFees - openPrice).TruncateToDecimalPlaces(3),
            Trend = (currentPrice / openPrice - 1).TruncateToDecimalPlaces(3),
            InvestedAmount = invested,
            UnitsPaid = (effectiveMoney / currentPrice).TruncateToDecimalPlaces(3),
            PercentageExposed = (effectiveMoney / requestingUser.Capital).TruncateToDecimalPlaces(3),
            WeekdayTax = _taxesCalculator.CalculateWeekDayTax(leverage, borrowedMoney, isBuy),
            WeekendTax = _taxesCalculator.CalculateWeekEndTax(leverage, borrowedMoney, isBuy),
        };
    }

    public async Task<PlaceMarketOrderRequest> PlaceOrder(ApplicationUser requestingUser,
        PlaceMarketOrderRequest marketOrder)
    {
        if (!DateTimeOffset.UtcNow.IsDateDuringStockMarketOpenTimeframe())
        {
            throw new StockMarketClosedException(
                "Stock market is closed, try again during business hours");
        }

        if (_transactionsRepository.ExistsTransaction(marketOrder.Token))
        {
            throw new OrderAlreadySubmitted("An error occured. You've already placed this order!");
        }

        if (marketOrder.InvestedAmount > requestingUser.Capital ||
            marketOrder.InvestedAmount == 0)
        {
            var extraMoneyNeeded = Math.Abs(marketOrder.InvestedAmount - requestingUser.Capital);

            throw new InsufficientFundsException($"Insufficient funds. Needs {extraMoneyNeeded}");
        }

        if (!TaxesConfig.Leverages.Contains(marketOrder.Leverage))
        {
            throw new InvalidLeverageValue("Invalid leverage value!");
        }

        bool leveragedTrade = (marketOrder.Leverage > 1) || (!marketOrder.IsBuy);
        bool stopLossTooBig = leveragedTrade && marketOrder.StopLossAmount >
            marketOrder.InvestedAmount * TaxesConfig.StopLossMaxPercent;
        bool stopLossNegative = marketOrder.StopLossAmount < 0;
        bool takeProfitNegative = marketOrder.TakeProfitAmount < 0;

        if (stopLossTooBig || stopLossNegative)
        {
            throw new InvalidStopLossValueForLeveragedTrade("Invalid stop loss amount for leveraged trade!\r\n" +
                                                            $"Stop loss should be between [1, {marketOrder.InvestedAmount * TaxesConfig.StopLossMaxPercent}]");
        }

        if (takeProfitNegative)
        {
            throw new InvalidTakeProfitValue("Take profit value invalid -> it was negative.");
        }

        var todaysPrices = _stockPricesRepository.GetTodaysPriceEvolution(marketOrder.Ticker);

        if (!todaysPrices.Any())
        {
            throw new NoStockPricesRecordedException("Our backend stocks prices store in unavailable." +
                                                     " Try again later");
        }

        var currentPrice = _stockPricesRepository.GetCurrentUnitPriceByStocksCompanyTicker(marketOrder.Ticker);
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
            ApplicationUserId = requestingUser.Id
        };

        await _usersRepository.OpenUserTransaction(requestingUser, transaction);

        return marketOrder;
    }

    public TradingContext GetTradingContext(ApplicationUser userRequesting)
    {
        return new TradingContext()
        {
            Funds = userRequesting.Capital,
        };
    }
}