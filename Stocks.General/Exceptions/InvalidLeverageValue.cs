using System;

namespace Stocks.General.Exceptions;

public class InvalidLeverageValue : Exception
{
    public InvalidLeverageValue(string? message) : base(message)
    {
    }
}