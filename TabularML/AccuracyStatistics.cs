namespace TabularML;

public class AccuracyStatistics
{
    public double RMSE { get; set; }
    public double MAE { get; set; }

    public override string ToString()
    {
        return $"Algorithm obtained RMSE: {RMSE} and MAE: {MAE}.";
    }
}