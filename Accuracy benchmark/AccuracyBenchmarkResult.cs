using StocksProcessing.ML;
using StocksProcessing.ML.Models;
using StocksProcessing.ML.Models.TimeSeries;

namespace Accuracy_benchmark;

[Serializable]
public class AccuracyBenchmarkResult
{
    public List<PredictionResult> ForecastedPrices { get; set; } = new();
    public AccuracyStatistics ComputedStatistics { get; set; }
    public List<TimestampPriceInputModel> Dataset { get; set; } = new();
    public DateTimeOffset DatasetBegin { get; set; }
    public DateTimeOffset DatasetEnd { get; set; }
    public DateTimeOffset RanDate { get; set; }
    public string Algorithm { get; set; }
    public string Ticker { get; set; }
    public double TestFraction { get; set; }

    public List<TimestampPriceInputModel> Actuals => Dataset
        .TakeLast((int) Math.Floor(Dataset.Count * TestFraction)).ToList();

    public static AccuracyBenchmarkResult FromBenchmarkTest(AccuracyStatistics accuracyStatistics,
        List<PredictionResult> results, string ticker, List<TimestampPriceInputModel> dataset,
        double testFraction)
    {
        return new()
        {
            Algorithm = "SingleSpectrumAnalysis",
            Ticker = ticker,
            ComputedStatistics = accuracyStatistics,
            ForecastedPrices = results,
            RanDate = DateTimeOffset.UtcNow,
            DatasetBegin = dataset.FirstOrDefault()?.Date ?? DateTimeOffset.FromUnixTimeSeconds(0),
            DatasetEnd = dataset.LastOrDefault()?.Date ?? DateTimeOffset.FromUnixTimeSeconds(0),
            TestFraction = testFraction,
            Dataset = dataset
        };
    }
}