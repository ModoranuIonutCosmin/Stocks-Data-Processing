using System;
using System.Runtime.Serialization;

namespace Stocks_Data_Processing.Exceptions
{
    [Serializable]
    public class CurrencyParseException : Exception
    {
        public CurrencyParseException()
        {
        }

        public CurrencyParseException(string message) : base(message)
        {
        }

        public CurrencyParseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CurrencyParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
