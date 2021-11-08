﻿using System;

namespace Stocks.General.Models
{
    public class DailySummary
    {
        public string Ticker { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal OpenValue { get; set; }
        public decimal CloseValue { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}