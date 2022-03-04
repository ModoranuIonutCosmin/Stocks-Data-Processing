using System;

namespace StocksProcessing.General.Exceptions;

public class InvalidPasswordResetLink : Exception
{
    public InvalidPasswordResetLink(string? message) : base(message)
    {
    }
}