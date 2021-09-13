using System;
using System.ComponentModel.DataAnnotations;

namespace StocksProccesing.Relational.Model
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(30)]
        public string PaymentProcessor { get; set; }

        [MaxLength(15)]
        public string CurrencyTicker { get; set; }

        public double Amount { get; set; }

        public DateTimeOffset DateFinalized { get; set; }
    }
}
