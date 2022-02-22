using System;

namespace StocksProcessing.API.Exceptions
{
    [Serializable]
    public class InvalidCompanyException : Exception
    {
        public InvalidCompanyException(string message) : base(message)
        {
        }
    }
}
