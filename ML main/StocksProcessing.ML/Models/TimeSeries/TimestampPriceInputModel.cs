using System;
using StocksProcessing.ML.Models.Tabular;

namespace StocksProcessing.ML.Models.TimeSeries;

public class TimestampPriceInputModel : IInputModel
{
    public DateTime Date { get; set; }
    public float Price { get; set; }

    public float GetLabel()
    {
        return Price;
    }

    public int GetLineSize()
    {
        return 1;
    }

    public DateTimeOffset GetObservationDate()
    {
        return Date;
    }
}