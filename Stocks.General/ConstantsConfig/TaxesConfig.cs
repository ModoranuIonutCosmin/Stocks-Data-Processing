using System.Collections.Generic;

namespace Stocks.General.ConstantsConfig
{
    public static class TaxesConfig
    {
        public const double BuyInterestRate = 0.08;
        public const double SellInterestRate = 0.03;
        public const double WeekendOvernightMultiplier = 3;
        public const double AverageStockMarketSpread = 0.00019;
        public const double SpreadFee = 0.0009; //per unit * price
        public const double StopLossMaxPercent = 0.5;
        public static double FullSpreadFees => AverageStockMarketSpread + SpreadFee;
        public static List<int> Leverages = new List<int> { 1, 2, 5 };
    }
}
