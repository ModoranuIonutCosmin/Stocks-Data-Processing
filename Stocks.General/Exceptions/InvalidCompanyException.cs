using System;

namespace StocksProcessing.General.Exceptions
{
    [Serializable]
    public class InvalidCompanyException : Exception
    {
        public InvalidCompanyException(string message) : base(message)
        {
        }
    }
}
