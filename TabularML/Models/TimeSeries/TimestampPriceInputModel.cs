using TabularML;

namespace StocksProcessing.ML.Models
{
    public class TimestampPriceInputModel : IInputModel
    {
        public DateTime Date { get; set; }

        public float Price { get; set; }
        public float GetLabel() => Price;
    }
}
