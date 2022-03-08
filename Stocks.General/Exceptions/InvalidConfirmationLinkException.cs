using System;

namespace StocksProcessing.General.Exceptions;

public class InvalidConfirmationLinkException : Exception
{
    public InvalidConfirmationLinkException(string? message) : base(message)
    {
    }
}