using System;

namespace StocksProcessing.ML.Models
{
    public class TimestampPriceOutputModel
    {
        public float[] ForecastedPrices { get; set; }

        public float[] LowerBoundPrices { get; set; }

        public float[] UpperBoundPrices { get; set; }
    }
}
