using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StocksProccesing.Relational.Model;

public class StocksTransaction
{
    [Key] public int Id { get; set; }

    [Column(TypeName = "varchar(40)")] public string UniqueActionStamp { get; set; }

    [Required] public bool Open { get; set; }

    [Required] public bool IsBuy { get; set; }

    [MaxLength(10)] public string Ticker { get; set; }

    [Required]
    [Column(TypeName = "decimal(20, 4)")]
    public decimal InvestedAmount { get; set; }

    [Required] public int Leverage { get; set; }

    [Required] public DateTimeOffset Date { get; set; }

    public DateTimeOffset ScheduledAutoClose { get; set; }

    [Required]
    [Column(TypeName = "decimal(20, 4)")]
    public decimal UnitSellPriceThen { get; set; }

    [Required]
    [Column(TypeName = "decimal(20, 4)")]
    public decimal UnitBuyPriceThen { get; set; }

    [Required]
    [Column(TypeName = "decimal(20, 4)")]
    public decimal StopLossAmount { get; set; }

    [Required]
    [Column(TypeName = "decimal(20, 4)")]
    public decimal TakeProfitAmount { get; set; }

    public string ApplicationUserId { get; set; }
}