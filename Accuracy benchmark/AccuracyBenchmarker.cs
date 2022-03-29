using StocksProcessing.ML;
using StocksProcessing.ML.Algorithms.Base;
using StocksProcessing.ML.Algorithms.TabularReduction;
using StocksProcessing.ML.Algorithms.TimeSeries;
using StocksProcessing.ML.Models;
using StocksProcessing.ML.Models.Tabular;
using StocksProcessing.ML.Models.TimeSeries;

namespace Accuracy_benchmark;

public class AccuracyBenchmarker
{
    private readonly List<TimestampPriceInputModel> _dataset;
    private List<TabularModelInput> _tabularDataset = new();
    private readonly TimeSpan _forecastDateInterval;

    public AccuracyBenchmarker(List<TimestampPriceInputModel> dataset, int tabularWindowSize,
        TimeSpan forecastDateInterval)
    {
        _dataset = dataset;
        _forecastDateInterval = forecastDateInterval;
        _tabularDataset = _dataset.Tabularize(tabularWindowSize)
            .ToTabularDataInputModel();
    }

    public async Task<AccuracyBenchmarkResult> BenchmarkSingleSpectrumAnalysis(string ticker, int horizon,
        double testFraction = 0.1)
    {
        IPredictionEngine ssaPredEngine = new SSAPredictionEngine(_dataset);

        var accuracyStatistics = await ssaPredEngine
            .EvaluateModel(horizon, testFraction, _forecastDateInterval);
        
        return AccuracyBenchmarkResult.FromBenchmarkTest(accuracyStatistics.accuracy,
            accuracyStatistics.predictions, ticker, _dataset, testFraction);
    }
    
    public async Task<AccuracyBenchmarkResult> BenchmarkFastForest(string ticker, int horizon,
        double testFraction = 0.1)
    {
        IPredictionEngine fastForestEngine
            = new TabularFastForestRegressionPredictionEngine(_tabularDataset);

        var accuracyStatistics 
            = await fastForestEngine.EvaluateModel(horizon, testFraction, _forecastDateInterval);
        
        return AccuracyBenchmarkResult.FromBenchmarkTest(accuracyStatistics.accuracy,
            accuracyStatistics.predictions, ticker, _dataset, testFraction);
    }
    
    public async Task<AccuracyBenchmarkResult> BenchmarkFastTreeTweedie(string ticker, int horizon,
        double testFraction = 0.1)
    {
        IPredictionEngine fastTreeTweedieEngine
            = new TabularFastTreeRegressionPredictionEngine(_tabularDataset);

        var accuracyStatistics 
            = await fastTreeTweedieEngine.EvaluateModel(horizon, testFraction, _forecastDateInterval);
        
        return AccuracyBenchmarkResult.FromBenchmarkTest(accuracyStatistics.accuracy,
            accuracyStatistics.predictions, ticker, _dataset, testFraction);
    }
    
    public async Task<AccuracyBenchmarkResult> BenchmarkSDCA(string ticker, int horizon,
        double testFraction = 0.1)
    {
        IPredictionEngine sdcaEngine
            = new TabularSdcaRegressionPredictionEngine(_tabularDataset);

        var accuracyStatistics 
            = await sdcaEngine.EvaluateModel(horizon, testFraction, _forecastDateInterval);
        
        return AccuracyBenchmarkResult.FromBenchmarkTest(accuracyStatistics.accuracy,
            accuracyStatistics.predictions, ticker, _dataset, testFraction);
    }
}