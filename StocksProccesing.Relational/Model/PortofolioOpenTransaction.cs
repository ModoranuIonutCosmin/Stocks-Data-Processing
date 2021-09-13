using System;
using System.ComponentModel.DataAnnotations;

namespace StocksProccesing.Relational.Model
{
    public class PortofolioOpenTransaction
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(16)]
        public string UniqueActionStamp { get; set; }

        [Required]
        public bool IsBuy { get; set; }

        [MaxLength(10)]
        public string Ticker { get; set; }

        [Required]
        public double InvestedSum { get; set; }

        [Required]
        public int Leverage { get; set; }

        [Required]
        public DateTimeOffset Date { get; set; }

        [Required]
        public double UnitSellPriceThen { get; set; }

        [Required]
        public double UnitBuyPriceThen { get; set; }

        [Required]
        public double StopLossAmount { get; set; }

        [Required]
        public double TakeProfitAmount { get; set; }

        public string ApplicationUserId { get; set; }
    }
}
