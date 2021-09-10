using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StocksProccesing.Relational.Model
{
    public class PortofolioOpenTransaction
    {
        [Key]
        public int Id { get; set; }

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
