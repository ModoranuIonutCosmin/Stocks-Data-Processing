using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator
{
    public interface IStockMarketProfitCalculator
    {
        decimal CalculateTransactionProfit(StocksTransaction transaction);
    }
}