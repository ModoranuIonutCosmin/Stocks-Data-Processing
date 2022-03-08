using Microsoft.ML.Data;

namespace TabularML;

public class TabularModelInput : IInputModel
{
    [VectorType(1439), LoadColumn(0, 1438),
    ColumnName("Features")]
    public float[] Features { get; set; }

    [LoadColumn(1439), ColumnName("Label")]
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