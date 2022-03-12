using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Features.StocksMarketMetricsCalculator
{
    public class StockMarketProfitCalculator : IStockMarketProfitCalculator
    {
        private readonly IStockPricesRepository _stockPricesRepository;
        private readonly IStockMarketDisplayPriceCalculator _stockDisplayPriceCalculator;

        public StockMarketProfitCalculator(
            IStockPricesRepository stockPricesRepository,
            IStockMarketDisplayPriceCalculator stockDisplayPriceCalculator)
        {
            _stockPricesRepository = stockPricesRepository;
            _stockDisplayPriceCalculator = stockDisplayPriceCalculator;
        }

        public decimal CalculateTransactionProfit(StocksTransaction transaction)
        {
            var currentSellPrice = _stockPricesRepository.GetCurrentUnitPriceByStocksCompanyTicker(transaction.Ticker);

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
