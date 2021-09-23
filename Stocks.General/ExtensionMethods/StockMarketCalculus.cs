using Stocks.General.ConstantsConfig;

namespace Stocks.General.ExtensionMethods
{
    public static class StockMarketCalculus
    {
        public static double CalculateBuyPrice(double sellPrice)
        {
            var buyPrice = sellPrice;

            buyPrice += sellPrice * (TaxesConfig.AverageStockMarketSpread + TaxesConfig.SpreadFee);

            return buyPrice;
        }
    }
}
