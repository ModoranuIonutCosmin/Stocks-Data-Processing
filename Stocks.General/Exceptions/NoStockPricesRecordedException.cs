using System;

namespace Stocks.General.Exceptions;

public class NoStockPricesRecordedException : Exception
{
    public NoStockPricesRecordedException(string? message) : base(message)
    {
    }
}