namespace StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator
{
    public interface IStockMarketDisplayPriceCalculator
    {
        decimal CalculateBuyPrice(decimal currentPrice, decimal leverage);
        decimal CalculateSellPrice(decimal currentPrice, decimal leverage = 1);
    }
}