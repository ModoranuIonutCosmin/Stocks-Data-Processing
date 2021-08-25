using System;
using System.Collections.Generic;
using System.Linq;

namespace StocksProcessing.ML
{
    public static class StatisticsHelpers
    {

        public static double CalculateRMSE(this List<float> predicted, List<float> actual)
        {
            var preds = new List<float>(predicted);
            var actuals = new List<float>(actual);

            int minLength = Math.Min(predicted.Count, actual.Count);

            for (int i = 0; i < minLength; i++)
            {
                preds[i] -= actuals[i];
                preds[i] *= preds[i];
            }

            return Math.Sqrt(preds.Average());
        }

        public static double CalculateMAE(this List<float> predicted, List<float> actual)
        {
            var preds = new List<float>(predicted);
            var actuals = new List<float>(actual);

            int minLength = Math.Min(predicted.Count, actual.Count);

            for (int i = 0; i < minLength; i++)
            {
                preds[i] = Math.Abs(preds[i] - actuals[i]);
            }

            return preds.Average();
        }
    }
}
