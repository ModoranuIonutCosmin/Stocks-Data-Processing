using System;
using Microsoft.ML.Data;

namespace StocksProcessing.ML.Models.Tabular;

public class TabularModelInput : IInputModel
{
    // [VectorType(79), LoadColumn(0, 78),
    // ColumnName("Features")]
    public float[] Features { get; set; }

    // [LoadColumn(79), ColumnName("Label")]
    public float Label { get; set; }
    
    public DateTimeOffset FirstFeatureDate { get; set; }

    public float GetLabel()
    {
        return this.Label;
    }
    public int GetLineSize()
        => Features.Length;

    public DateTimeOffset GetObservationDate()
    {
        return this.FirstFeatureDate;
    }
}