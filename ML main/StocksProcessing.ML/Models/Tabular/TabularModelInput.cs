using System;

namespace StocksProcessing.ML.Models.Tabular;

public class TabularModelInput : IInputModel
{
    // [VectorType(79), LoadColumn(0, 78),
    // ColumnName("Features")]
    public float[] Features { get; set; }

    // [LoadColumn(79), ColumnName("Label")]
    public float Label { get; set; }

    public DateTimeOffset LastFeatureDate { get; set; }

    public float GetLabel()
    {
        return Label;
    }

    public int GetLineSize()
    {
        return Features.Length;
    }

    public DateTimeOffset GetObservationDate()
    {
        return LastFeatureDate;
    }
}