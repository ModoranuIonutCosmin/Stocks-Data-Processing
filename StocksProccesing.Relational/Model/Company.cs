﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StocksProccesing.Relational.Model
{
    public class Company
    {
        [Key]
        [MaxLength(10)]
        public string Ticker { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        [Column(TypeName = "VARCHAR(128)")]
        public string UrlLogo { get; set; }

        public List<StocksPriceData> PricesData { get; set; } = new List<StocksPriceData>();
        public List<StocksOHLC> SummariesData { get; set; } = new List<StocksOHLC>();
    }
}
