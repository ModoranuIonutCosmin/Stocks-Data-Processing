using System;
using System.ComponentModel.DataAnnotations;

namespace StocksProccesing.Relational.Model
{
    public class StocksPriceData : IComparable<StocksPriceData>
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public bool Prediction { get; set; }

        [Required]
        public DateTimeOffset Date { get; set; }

        [MaxLength(10)]
        public string CompanyTicker { get; set; }

        public Company Company { get; set; }

        public int CompareTo(StocksPriceData other)
        {
            return Price.CompareTo(other.Price);
        }
    }
}
