using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StocksProccesing.Relational.Model;

public class Order
{
    [Key] public int Id { get; set; }

    [MaxLength(30)] public string PaymentProcessor { get; set; }

    [MaxLength(15)] public string CurrencyTicker { get; set; }

    [Column(TypeName = "decimal(20, 4)")] public decimal Amount { get; set; }

    public DateTimeOffset DateFinalized { get; set; }
}