using System;

namespace StocksProcessing.ML
{
    public class PredictionResult
    {
        public DateTimeOffset Date { get; set; }

        public float Price { get; set; }

        public override string ToString()
        {
            return $"Date: {Date}, Price: {Price}";
        }
    }
}