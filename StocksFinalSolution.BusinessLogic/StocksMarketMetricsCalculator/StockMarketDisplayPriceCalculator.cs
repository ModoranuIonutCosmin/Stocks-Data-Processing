using Stocks.General.ConstantsConfig;
using Stocks.General.ExtensionMethods;

namespace StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator
{
    public class StockMarketDisplayPriceCalculator : IStockMarketDisplayPriceCalculator
    {
        private readonly IPricesDisparitySimulator _pricesDisparitySim;

        public StockMarketDisplayPriceCalculator(IPricesDisparitySimulator pricesDisparitySim)
        {
            _pricesDisparitySim = pricesDisparitySim;
        }
        public decimal CalculateBuyPrice(decimal currentPrice, decimal leverage = 1)
        {
            var leverageFees = leverage == 1 ? 0 : TaxesConfig.SpreadFee;
            var buyPrice = currentPrice;

            buyPrice += currentPrice * (_pricesDisparitySim.ComputeSpreadAmount() + leverageFees);

            return buyPrice.TruncateToDecimalPlaces(3);
        }

        public decimal CalculateSellPrice(decimal currentPrice, decimal leverage = 1)
        {
            return currentPrice + currentPrice * TaxesConfig.SpreadFee;
        }

        //public double CalculatePriceTrend(List<>)
        //{
        //    var buyPrice = sellPrice;

        //    buyPrice += sellPrice * (_pricesDisparitySim.ComputeSpreadAmount() + TaxesConfig.SpreadFee);

        //    return buyPrice;
        //}

    }
}
