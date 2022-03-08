using System;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Services
{
    public interface IStockMarketOrderTaxesCalculator
    {
        decimal CalculateTaxes(StocksTransaction transaction, DateTimeOffset lastCollected);
        decimal CalculateWeekDayTax(decimal leverage, decimal effectiveMoney, bool isBuy);
        decimal CalculateWeekEndTax(decimal leverage, decimal effectiveMoney, bool isBuy);
    }
}