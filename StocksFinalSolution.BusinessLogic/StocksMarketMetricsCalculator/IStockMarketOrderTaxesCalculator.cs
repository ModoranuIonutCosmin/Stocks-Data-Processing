using StocksProccesing.Relational.Model;
using System;

namespace StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator
{
    public interface IStockMarketOrderTaxesCalculator
    {
        decimal CalculateTaxes(StocksTransaction transaction, DateTimeOffset lastCollected);
        decimal CalculateWeekDayTax(decimal leverage, decimal effectiveMoney, bool isBuy);
        decimal CalculateWeekEndTax(decimal leverage, decimal effectiveMoney, bool isBuy);
    }
}