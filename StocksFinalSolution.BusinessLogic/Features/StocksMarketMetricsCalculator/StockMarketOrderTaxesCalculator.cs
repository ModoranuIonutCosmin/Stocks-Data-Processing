using System;
using Stocks.General.ConstantsConfig;
using Stocks.General.ExtensionMethods;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Features.StocksMarketMetricsCalculator
{
    public class StockMarketOrderTaxesCalculator : IStockMarketOrderTaxesCalculator
    {

        public decimal CalculateWeekDayTax(decimal leverage, decimal effectiveMoney, bool isBuy)
        {
            var interestRate = isBuy ? TaxesConfig.BuyInterestRate : TaxesConfig.SellInterestRate;

            return leverage == 1
                ? 0
                : ((interestRate
                    + BankExchangeConsts.LiborOneMonthRatio) / 365 * effectiveMoney).TruncateToDecimalPlaces(3);
        }

        public decimal CalculateWeekEndTax(decimal leverage, decimal effectiveMoney, bool isBuy)
        {
            return CalculateWeekDayTax(leverage, effectiveMoney, isBuy) * 3;
        }

        public decimal CalculateTaxes(StocksTransaction transaction, DateTimeOffset lastCollected)
        {
            var currentDate = DateTimeOffset.UtcNow;

            var dateTransactionUpdated = lastCollected < transaction.Date ? transaction.Date
                                                                                 : lastCollected;

            var weekDays = DateTimeOffsetHelpers.GetBusinessDays(dateTransactionUpdated, currentDate);
            var weekendDays = (decimal)currentDate.Subtract(dateTransactionUpdated).TotalDays - weekDays;

            var borrowedMoney = transaction.Leverage * transaction.InvestedAmount - transaction.InvestedAmount;

            var weekdayTax = CalculateWeekDayTax(transaction.Leverage, borrowedMoney, transaction.IsBuy);
            var weekendTax = CalculateWeekEndTax(transaction.Leverage, borrowedMoney, transaction.IsBuy);

            return weekDays * weekdayTax + weekendDays * weekendTax;
        }
    }
}
