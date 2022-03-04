using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StocksProccesing.Relational.Model
{
    public class StocksPriceData : IComparable<StocksPriceData>
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(20, 4)")]
        public decimal Price { get; set; }

        [Required]
        public bool Prediction { get; set; }

        [Required]
        public DateTimeOffset Date { get; set; }

        [MaxLength(10)]
        public string CompanyTicker { get; set; }
        [MaxLength(100)]
        public string AlgorithmUsed { get; set; }
        public int CompareTo(StocksPriceData other)
        {
            return Price.CompareTo(other.Price);
        }
    }
}
