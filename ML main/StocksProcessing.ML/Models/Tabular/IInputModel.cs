using System;

namespace StocksProcessing.ML.Models.Tabular;

public interface IInputModel
{
    public float GetLabel();
    public int GetLineSize();

    public DateTimeOffset GetObservationDate();
}