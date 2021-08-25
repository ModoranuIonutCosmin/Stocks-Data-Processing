using System;
using System.Collections.Generic;

namespace StocksProcessing.ML.Models
{
    public class SeparatedPricesDataInputModel
    {
        public IEnumerable<PriceDataInputModel> TrainData { get; set; }
        public IEnumerable<PriceDataInputModel> TestData { get; set; }
    }
}
