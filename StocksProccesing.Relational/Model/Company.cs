﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace StocksProccesing.Relational.Model
{
    public class Company
    {

        [Key]
        [MaxLength(10)]
        public string Ticker { get; set; }


        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(350)]
        public string UrlLogo { get; set; }

        public List<StocksPriceData> PricesData { get; set; } = new List<StocksPriceData>();
    }
}
