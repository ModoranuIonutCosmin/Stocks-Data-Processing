namespace StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator
{
    public interface IStockMarketOrderTaxesCalculator
    {
        decimal CalculateWeekDayTax(decimal leverage, decimal effectiveMoney, bool isBuy);
        decimal CalculateWeekEndTax(decimal leverage, decimal effectiveMoney, bool isBuy);
    }
}