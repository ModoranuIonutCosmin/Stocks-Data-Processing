using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stocks.General.ExtensionMethods;
using Stocks.General.Models.Transactions;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Features.StocksMarketMetricsCalculator;

public class TransactionSummaryCalculator : ITransactionSummaryCalculator
{
    private readonly ICompaniesRepository companiesRepository;
    private readonly IStockMarketDisplayPriceCalculator displayPriceCalculator;
    private readonly IStockPricesRepository pricesRepository;

    public TransactionSummaryCalculator(ICompaniesRepository companiesRepository,
        IStockPricesRepository pricesRepository,
        IStockMarketDisplayPriceCalculator displayPriceCalculator)
    {
        this.companiesRepository = companiesRepository;
        this.pricesRepository = pricesRepository;
        this.displayPriceCalculator = displayPriceCalculator;
    }

    public async Task<List<TransactionSummary>> AggregateOpenTransactionsDataByCompaniesInfoAsync(
        List<StocksTransaction> transactions)
    {
        var companiesData = await companiesRepository.GetAllAsync();

        var allOpenTransactions = transactions
            .Select(e =>
            {
                var sellPrice = pricesRepository.GetCurrentUnitPriceByStocksCompanyTicker(e.Ticker);
                var buyPrice = displayPriceCalculator.CalculateBuyPrice(sellPrice, e.Leverage);
                var currentPrice = e.IsBuy ? sellPrice : buyPrice;
                var initialPrice = e.IsBuy ? e.UnitBuyPriceThen : e.UnitSellPriceThen;
                var unitsPurchased = e.InvestedAmount / (e.IsBuy ? e.UnitBuyPriceThen : e.UnitSellPriceThen);
                var profitOrLoss = ((currentPrice - initialPrice) * unitsPurchased *
                                    (!e.IsBuy ? -1 : 1)).TruncateToDecimalPlaces(3);
                var profitOrLossPercentage =
                    (profitOrLoss / (initialPrice * unitsPurchased)).TruncateToDecimalPlaces(3);

                return new TransactionFullInfo
                {
                    CurrentPrice = currentPrice,
                    InitialPrice = initialPrice,
                    InvestedAmount = e.InvestedAmount,
                    IsBuy = e.IsBuy,
                    Ticker = e.Ticker,
                    UnitsPurchased = unitsPurchased,
                    ProfitOrLoss = profitOrLoss,
                    ProfitOrLossPercentage = profitOrLossPercentage
                };
            })
            .GroupBy(c => c.Ticker)
            .Select(g => new TransactionSummary
            {
                Ticker = g.Key,
                AverageInitial = g.Average(e => e.InitialPrice).TruncateToDecimalPlaces(3),
                TotalInvested = g.Sum(e => e.InvestedAmount).TruncateToDecimalPlaces(3),
                TotalPl = g.Sum(e => e.ProfitOrLoss).TruncateToDecimalPlaces(3),
                TotalPlPercentage = (g.Sum(e => e.ProfitOrLoss) / g.Sum(e => e.InvestedAmount) * 100)
                    .TruncateToDecimalPlaces(3),
                TotalUnits = g.Sum(e => e.UnitsPurchased).TruncateToDecimalPlaces(3),
                Value = g.Sum(e => e.UnitsPurchased * e.InitialPrice + e.ProfitOrLoss).TruncateToDecimalPlaces(3)
            })
            .Join(companiesData, e => e.Ticker, e => e.Ticker,
                (transaction, company) =>
                {
                    transaction.UrlLogo = company.UrlLogo;
                    transaction.Name = company.Name;
                    transaction.Description = company.Description;

                    return transaction;
                });

        return allOpenTransactions.ToList();
    }

    public AllTransactionsDetailed AggregateOpenTransactionsDataForSingleCompany(List<StocksTransaction> transactions,
        string ticker)
    {
        var companyData = companiesRepository.GetCompanyData(ticker);
        var openTransactions = transactions.Where(e => e.Open == true).ToList();
        var scheduledTransactions = transactions.Where(e => !e.Open && 
            e.ScheduledAutoOpen != default &&
            e.ScheduledAutoOpen >= DateTimeOffset.UtcNow).ToList();
        var closedTransactions = transactions.Where(e => !e.Open &&
        (e.ScheduledAutoOpen == default || e.ScheduledAutoOpen < DateTimeOffset.UtcNow)).ToList();


        var result = new AllTransactionsDetailed
        {
            OpenTransactions = CalculateTransactionDetailedInfo(openTransactions),
            ClosedTransactions = CalculateTransactionDetailedInfo(closedTransactions),
            ScheduledTransactions = CalculateTransactionDetailedInfo(scheduledTransactions),
            UrlLogo = companyData.UrlLogo,
            Name = companyData.Name,
            Description = companyData.Description,
            Ticker = ticker,

        };

        return result;
    }

    private List<TransactionFullInfo> CalculateTransactionDetailedInfo(List<StocksTransaction> transactions)
    {
        return transactions.Select(e =>
        {
            var currentSellPrice = pricesRepository.GetCurrentUnitPriceByStocksCompanyTicker(e.Ticker);

            var currentPrice = e.IsBuy
                ? currentSellPrice
                : displayPriceCalculator.CalculateBuyPrice(currentSellPrice, e.Leverage);
            var isCFD = e.Leverage > 1 || !e.IsBuy;
            var initialPrice = e.IsBuy ? e.UnitBuyPriceThen : e.UnitSellPriceThen;
            var unitsPurchased = e.InvestedAmount / (e.IsBuy ? e.UnitBuyPriceThen : e.UnitSellPriceThen);
            var profitOrLoss = ((currentPrice - initialPrice) * unitsPurchased *
                                (!e.IsBuy ? -1 : 1)).TruncateToDecimalPlaces(3);
            var profitOrLossPercentage =
                (profitOrLoss / (initialPrice * unitsPurchased)).TruncateToDecimalPlaces(3);

            return new TransactionFullInfo
            {
                Id = e.Id,
                Ticker = e.Ticker,
                CurrentPrice = currentPrice.TruncateToDecimalPlaces(3),
                InitialPrice = (e.IsBuy ? e.UnitBuyPriceThen : e.UnitSellPriceThen).TruncateToDecimalPlaces(3),
                IsBuy = e.IsBuy,
                Leverage = e.Leverage,
                InvestedAmount = e.InvestedAmount,
                UnitsPurchased = (e.InvestedAmount / (e.IsBuy ? e.UnitBuyPriceThen : e.UnitSellPriceThen))
                    .TruncateToDecimalPlaces(3),
                StopLossAmount = e.StopLossAmount.TruncateToDecimalPlaces(3),
                TakeProfitAmount = e.TakeProfitAmount.TruncateToDecimalPlaces(3),
                Date = e.Date,
                IsCFD = isCFD,
                ProfitOrLoss = profitOrLoss,
                ProfitOrLossPercentage = profitOrLossPercentage,
                ScheduledAutoClose = e.ScheduledAutoClose,
                ScheduledAutoOpen = e.ScheduledAutoOpen
            };
        }).ToList();
    }
}
