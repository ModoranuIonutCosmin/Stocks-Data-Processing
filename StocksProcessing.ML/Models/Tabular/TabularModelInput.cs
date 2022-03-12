using Microsoft.ML.Data;

namespace StocksProcessing.ML.Models.Tabular;

public class TabularModelInput : IInputModel
{
    [VectorType(79), LoadColumn(0, 78),
    ColumnName("Features")]
    public float[] Features { get; set; }

    [LoadColumn(79), ColumnName("Label")]
    public float Next { get; set; }

    public float GetLabel()
    {
        return this.Next;
    }
}

public interface IInputModel
{
    public float GetLabel();
}