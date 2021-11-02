using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Helpers;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator
{
    public class StockMarketProfitCalculator : IStockMarketProfitCalculator
    {
        private readonly IStockMarketDisplayPriceCalculator _stockDisplayPriceCalculator;

        public StockMarketProfitCalculator(StocksMarketContext dbContext,
            IStockMarketDisplayPriceCalculator stockDisplayPriceCalculator)
        {
            _dbContext = dbContext;
            _stockDisplayPriceCalculator = stockDisplayPriceCalculator;
        }

        public StocksMarketContext _dbContext { get; }

        public decimal CalculateTransactionProfit(StocksTransaction transaction)
        {
            var currentSellPrice = _dbContext.GatherCurrentPriceByCompany(transaction.Ticker);

            var currentBuyPrice = _stockDisplayPriceCalculator.CalculateBuyPrice(currentSellPrice, transaction.Leverage);

            var initialSellPrice = transaction.UnitSellPriceThen;
            var initialBuyPrice = transaction.UnitBuyPriceThen;

            decimal profitOrLoss;

            if (transaction.IsBuy)
            {
                profitOrLoss = (currentSellPrice - initialBuyPrice) * (transaction.InvestedAmount / initialBuyPrice);
            }
            else
            {
                profitOrLoss = (initialSellPrice - currentBuyPrice) * (transaction.InvestedAmount / initialSellPrice);
            }

            return profitOrLoss;
        }
    }
}
