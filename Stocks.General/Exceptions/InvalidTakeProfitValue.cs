using System;

namespace Stocks.General.Exceptions;

public class InvalidTakeProfitValue : Exception
{
    public InvalidTakeProfitValue(string? message) : base(message)
    {
    }
}