using System;

namespace Stocks.General.Exceptions;

public class InvalidStopLossValueForLeveragedTrade : Exception
{
    public InvalidStopLossValueForLeveragedTrade(string? message) : base(message)
    {
        
    }
}