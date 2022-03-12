using System;

namespace Stocks.General.Exceptions;

public class InvalidTransactionOwner: Exception
{
    public InvalidTransactionOwner(string? message) : base(message)
    {
    }
}