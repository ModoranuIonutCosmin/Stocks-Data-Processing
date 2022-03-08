using System.Collections.Generic;
using StocksProccesing.Relational.Model;

namespace Stocks.General.Models
{
    public class StocksPredictionsPaginatedDTO
    {
        public List<StocksPriceData> Predictions { get; set; }
        public string Algorithm { get; set; }
        public int Page { get; set; }
        public int Count { get; set; }
        public int TotalCount { get; set; }
    }
}