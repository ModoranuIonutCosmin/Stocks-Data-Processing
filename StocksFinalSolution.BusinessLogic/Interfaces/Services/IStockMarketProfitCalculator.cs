using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Services;

public interface IStockMarketProfitCalculator
{
    decimal CalculateTransactionProfit(StocksTransaction transaction);
}