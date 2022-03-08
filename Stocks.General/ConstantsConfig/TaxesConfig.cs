using System.Collections.Generic;

namespace Stocks.General.ConstantsConfig
{
    public static class TaxesConfig
    {
        public const decimal BuyInterestRate = 0.08m;
        public const decimal SellInterestRate = 0.03m;
        public const decimal WeekendOvernightMultiplier = 3;
        public const decimal AverageStockMarketSpread = 0.00019m;
        public const decimal SpreadFee = 0.0009m; //per unit * price
        public const decimal StopLossMaxPercent = 0.5m;
        public static decimal FullSpreadFees => AverageStockMarketSpread + SpreadFee;
        public static IReadOnlyCollection<int> Leverages = new List<int> { 1, 2, 5 };
    }
}
