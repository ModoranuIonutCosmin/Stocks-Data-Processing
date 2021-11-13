namespace StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator
{
    public interface IStockMarketDisplayPriceCalculator
    {
        decimal CalculateBuyPrice(decimal currentPrice, decimal leverage = 1);
        decimal CalculateSellPrice(decimal currentPrice, decimal leverage = 1);
    }
}