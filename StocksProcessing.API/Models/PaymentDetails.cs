using System;

namespace StocksProcessing.API.Models
{
    public class PaymentDetails
    {
        public string PaymentHandler { get; set; }

        public string InitialCurrencyTicker { get; set; } = "USD";

        public double Amount { get; set; }

        public DateTimeOffset PaymentDate { get; set; }
    }
}
