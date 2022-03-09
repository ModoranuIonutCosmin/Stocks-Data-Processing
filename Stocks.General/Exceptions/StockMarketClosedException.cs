using System;

namespace Stocks.General.Exceptions;

public class StockMarketClosedException : Exception
{
    public StockMarketClosedException(string? message) : base(message)
    {
    }
}