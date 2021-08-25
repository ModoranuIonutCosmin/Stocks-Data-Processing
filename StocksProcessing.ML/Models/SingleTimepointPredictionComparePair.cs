using System;

namespace StocksProcessing.ML.Models
{
    public class SingleTimepointPredictionComparePair
    {
        public DateTimeOffset DateTime { get;set;  }
        public float ActualValue { get; set; }
        public float PredictedValue { get; set; }
        public float Difference { get => PredictedValue - ActualValue;  }

        public override string ToString()
        {
            return $"Data: {DateTime} \n" +
                $"Actual value: {ActualValue} \n" +
                $"Predicted value: {PredictedValue} \n" +
                $"Difference: {Difference}";
        }
    }
}
