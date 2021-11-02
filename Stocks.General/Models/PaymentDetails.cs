using System;

namespace Stocks.General.Models
{
    public class PaymentDetails
    {
        public string PaymentHandler { get; set; }

        public string InitialCurrencyTicker { get; set; } = "USD";

        public decimal Amount { get; set; }

        public DateTimeOffset PaymentDate { get; set; }
    }
}
