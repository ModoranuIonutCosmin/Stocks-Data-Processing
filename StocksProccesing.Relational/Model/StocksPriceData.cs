using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StocksProccesing.Relational.Model
{
    public class StocksPriceData
    {
        [Key]
        public int Id { get; set; }

        public double Price { get; set; }

        public bool Prediction { get; set; }

        public DateTimeOffset Date { get; set; }

        [MaxLength(10)]
        public string CompanyTicker { get; set; }
    }
}
