using System;
using System.Collections.Generic;
using System.Linq;

namespace StocksProcessing.ML;

public static class StatisticsHelpers
{
    public static double CalculateRMSE(this IEnumerable<float> predicted, IEnumerable<float> actual)
    {
        var preds = new List<float>(predicted);
        var actuals = new List<float>(actual);

        var minLength = Math.Min(preds.Count, actuals.Count);

        return Math.Sqrt(preds.Zip(actuals).Take(minLength)
            .Select(obs => Math.Pow(obs.First - obs.Second, 2)).Average());
    }

    public static double CalculateMAE(this IEnumerable<float> predicted, IEnumerable<float> actual)
    {
        var predictedList = predicted.ToList();
        var actualList = actual.ToList();
        var minLength = Math.Min(actualList.Count, predictedList.Count);

        return actualList.Zip(predictedList).Take(minLength)
            .Select(obs => Math.Abs(obs.First - obs.Second))
            .Average();
    }
}