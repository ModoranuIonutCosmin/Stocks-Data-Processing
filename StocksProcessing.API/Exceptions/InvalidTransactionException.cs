using System;

namespace StocksProcessing.API.Exceptions
{
    [Serializable]
    public class InvalidTransactionException : Exception
    {
        public InvalidTransactionException(string message) : base(message)
        {
        }
    }
}
