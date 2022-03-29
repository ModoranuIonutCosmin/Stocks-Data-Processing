using System;

namespace StocksProcessing.ML
{
    public class PredictionResult : IComparable<PredictionResult>
    {
        public DateTimeOffset Date { get; set; }
        public float Price { get; set; }

        public int CompareTo(PredictionResult other)
        {
            return Price.CompareTo(other.Price);
        }

        public override string ToString()
        {
            return $"Date: {Date}, Price: {Price}";
        }
    }
}