using System;

namespace Stocks.General.Exceptions;

public class OrderAlreadySubmitted: Exception
{
    public OrderAlreadySubmitted(string? message) : base(message)
    {
    }
}