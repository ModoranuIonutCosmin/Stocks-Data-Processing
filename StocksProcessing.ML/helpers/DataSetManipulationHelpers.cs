using StocksProcessing.ML.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using StocksProcessing.ML.Models.TimeSeries;

namespace StocksProcessing.ML
{
    public static class DataSetManipulationHelpers
    {
        public static SeparatedDataset<TM> SeparateDataSet<TM>
            (this IEnumerable<TM> allData, double testFraction)

        {
            double trainFraction = 1 - testFraction;

            IEnumerable<TM> trainList
                = allData.Take((int)(allData.Count() * trainFraction));
            IEnumerable<TM> testList
                = allData.Skip(Math.Max(0, (int)(allData.Count() * trainFraction)));

            return new SeparatedDataset<TM>()
            {
                TrainData = trainList,
                TestData = testList
            };
        }

        public static List<List<float>> Tabularize(this IEnumerable<TimestampPriceInputModel> allData, int windowSize)
        {
            int datasetOffset = 0;
            List<TimestampPriceInputModel> dataset = new List<TimestampPriceInputModel>(allData);
            List<List<float>> features = new List<List<float>>();

            List<float> chunk = new List<float>();

            while (datasetOffset + windowSize <= dataset.Count)
            {
                chunk = dataset.GetRange(datasetOffset, windowSize)
                .Select(e => e.Price).ToList();

                datasetOffset++;

                if (windowSize != chunk.Count)
                {
                    break;
                }

                features.Add(chunk);
            }

            return features;
        }
    }
}
