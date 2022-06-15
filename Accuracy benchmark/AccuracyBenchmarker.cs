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
    private readonly TimeSpan _forecastDateInterval;
    private readonly List<TabularModelInput> _tabularDataset = new();

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
            accuracyStatistics.predictions, ticker, _dataset, testFraction: testFraction, "SSA");
    }

    public async Task<AccuracyBenchmarkResult> BenchmarkFastForest(string ticker, int horizon,
        double testFraction = 0.1)
    {
        IPredictionEngine fastForestEngine
            = new TabularFastForestRegressionPredictionEngine(_tabularDataset);

        var accuracyStatistics
            = await fastForestEngine.EvaluateModel(horizon, testFraction, _forecastDateInterval);

        return AccuracyBenchmarkResult.FromBenchmarkTest(accuracyStatistics.accuracy,
            accuracyStatistics.predictions, ticker, _dataset, testFraction, "FastForest");
    }

    public async Task<AccuracyBenchmarkResult> BenchmarkFastTreeTweedie(string ticker, int horizon,
        double testFraction = 0.1)
    {
        IPredictionEngine fastTreeTweedieEngine
            = new TabularFastTreeRegressionPredictionEngine(_tabularDataset);

        var accuracyStatistics
            = await fastTreeTweedieEngine.EvaluateModel(horizon, testFraction, _forecastDateInterval);

        return AccuracyBenchmarkResult.FromBenchmarkTest(accuracyStatistics.accuracy,
            accuracyStatistics.predictions, ticker, _dataset, testFraction, "FastTreeTweedie");
    }

    public async Task<AccuracyBenchmarkResult> BenchmarkSDCA(string ticker, int horizon,
        double testFraction = 0.1)
    {
        IPredictionEngine sdcaEngine
            = new TabularSdcaRegressionPredictionEngine(_tabularDataset);

        var accuracyStatistics
            = await sdcaEngine.EvaluateModel(horizon, testFraction, _forecastDateInterval);

        return AccuracyBenchmarkResult.FromBenchmarkTest(accuracyStatistics.accuracy,
            accuracyStatistics.predictions, ticker, _dataset, testFraction, "SDCA");
    }


    public async Task<List<AccuracyStatistics>> BenchmarkFastForestMultiple(string ticker, int horizon, int times = 30, double testFraction = 0.1)
    {
        Console.WriteLine("Battery of tests for Fast forest====");

        IPredictionEngine fastForestEngine = new TabularFastForestRegressionPredictionEngine(_tabularDataset);

        var results = new List<AccuracyStatistics>();
        AccuracyStatistics average = new AccuracyStatistics();

        for (int time = 0; time < times; time++)
        {
            fastForestEngine = new TabularFastForestRegressionPredictionEngine(_tabularDataset);

            var (accuracy, predictions) =
                await fastForestEngine.EvaluateModel(horizon, testFraction, _forecastDateInterval);

            Console.WriteLine($"[{time}] RMSE: {accuracy.RMSE} MAE: {accuracy.MAE}");

            results.Add(accuracy);
        }

        ReportStats(results);

        Console.WriteLine("Battery of tests for Fast forest====");

        return results;
    }


    public async Task<List<AccuracyStatistics>> BenchmarkFastTreeMultiple(string ticker, int horizon, int times = 30, double testFraction = 0.1)
    {
        Console.WriteLine("Battery of tests for Fast tree====");

        IPredictionEngine fastForestEngine = new TabularFastTreeRegressionPredictionEngine(_tabularDataset);

        var results = new List<AccuracyStatistics>();
        AccuracyStatistics average = new AccuracyStatistics();

        for (int time = 0; time < times; time++)
        {
            fastForestEngine = new TabularFastTreeRegressionPredictionEngine(_tabularDataset);

            var (accuracy, predictions) =
                await fastForestEngine.EvaluateModel(horizon, testFraction, _forecastDateInterval);

            Console.WriteLine($"[{time}] RMSE: {accuracy.RMSE} MAE: {accuracy.MAE}");

            results.Add(accuracy);
        }

        ReportStats(results);


        Console.WriteLine("Battery of tests for Fast tree====");

        return results;
    }

    public async Task<List<AccuracyStatistics>> BenchmarkSdcaMultiple(string ticker, int horizon, int times = 30, double testFraction = 0.1)
    {
        Console.WriteLine("Battery of tests for SDCA====");

        IPredictionEngine fastForestEngine = new TabularSdcaRegressionPredictionEngine(_tabularDataset);

        var results = new List<AccuracyStatistics>();
        AccuracyStatistics average = new AccuracyStatistics();

        for (int time = 0; time < times; time++)
        {
            fastForestEngine = new TabularSdcaRegressionPredictionEngine(_tabularDataset);

            var (accuracy, predictions) =
                await fastForestEngine.EvaluateModel(horizon, testFraction, _forecastDateInterval);

            Console.WriteLine($"[{time}] RMSE: {accuracy.RMSE} MAE: {accuracy.MAE}");

            results.Add(accuracy);
        }

        ReportStats(results);

        Console.WriteLine("Battery of tests for SDCA====");

        return results;
    }

    private void ReportStats(List<AccuracyStatistics> results)
    {
        var average = new AccuracyStatistics();

        average.MAE = results.Average(r => r.MAE);
        average.RMSE = results.Average(r => r.RMSE);

        var worst = results.MaxBy(e => e.RMSE);
        var best = results.MinBy(e => e.RMSE);

        Console.WriteLine($"[AVG] RMSE: {average.RMSE} MAE: {average.MAE}");
        Console.WriteLine($"[BEST] RMSE: {best.RMSE} MAE: {best.MAE}");
        Console.WriteLine($"[WORST] RMSE: {worst.RMSE} MAE: {worst.MAE}");
    }   
}