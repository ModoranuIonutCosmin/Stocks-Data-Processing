using Stocks.General.ConstantsConfig;
using Stocks.General.ExtensionMethods;

namespace StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator
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
    }
}
