namespace StocksFinalSolution.BusinessLogic.Interfaces.Services
{
    public interface IStockMarketDisplayPriceCalculator
    {
        decimal CalculateBuyPrice(decimal currentPrice, decimal leverage = 1);
        decimal CalculateSellPrice(decimal currentPrice, decimal leverage = 1);
    }
}