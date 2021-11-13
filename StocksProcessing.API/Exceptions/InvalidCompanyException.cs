using System;

namespace StocksProcessing.API.Exceptions
{
    public class InvalidCompanyException : Exception
    {
        public InvalidCompanyException(string message) : base(message)
        {
        }
    }
}
