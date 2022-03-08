using System;

namespace StocksProcessing.General.Exceptions
{
    [Serializable]
    public class InvalidTransactionException : Exception
    {
        public InvalidTransactionException(string message) : base(message)
        {
        }
    }
}
