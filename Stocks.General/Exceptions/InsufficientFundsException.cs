using System;

namespace Stocks.General.Exceptions;

public class InsufficientFundsException : Exception
{
    public InsufficientFundsException(string? message) : base(message)
    {
    }
}