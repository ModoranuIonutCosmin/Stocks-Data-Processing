using System;
using System.Runtime.Serialization;

namespace Stocks_Data_Processing.Exceptions;

[Serializable]
public class ScrapeNoElementException : Exception
{
    public ScrapeNoElementException()
    {
    }

    public ScrapeNoElementException(string message) : base(message)
    {
    }

    public ScrapeNoElementException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected ScrapeNoElementException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}