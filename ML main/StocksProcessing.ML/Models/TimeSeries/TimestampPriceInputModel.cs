using System;
using StocksProcessing.ML.Models.Tabular;

namespace StocksProcessing.ML.Models.TimeSeries
{
    public class TimestampPriceInputModel : IInputModel
    {
        public DateTime Date { get; set; }
        public float Price { get; set; }
        public float GetLabel() => Price;

        public int GetLineSize()
            => 1;
        
        public DateTimeOffset GetObservationDate()
        {
            return Date;
        }
    }
}
