namespace StocksProcessing.ML.Models.TimeSeries;

public class TimestampPriceOutputModel
{
    public float[] ForecastedPrices { get; set; }

    public float[] LowerBoundPrices { get; set; }

    public float[] UpperBoundPrices { get; set; }
}