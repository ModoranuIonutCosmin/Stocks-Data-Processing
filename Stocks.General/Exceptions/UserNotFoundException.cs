using System;

namespace StocksProcessing.General.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(string? message) : base(message)
    {
    }
}