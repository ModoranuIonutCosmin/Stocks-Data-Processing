using StocksProcessing.ML.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using StocksProcessing.ML.Models.Tabular;
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

        public static List<List<TimestampPriceInputModel>> Tabularize(this IEnumerable<TimestampPriceInputModel> allData, int windowSize)
        {
            int datasetOffset = 0;
            List<TimestampPriceInputModel> dataset = new List<TimestampPriceInputModel>(allData);
            List<List<TimestampPriceInputModel>> featuresGrouped = new List<List<TimestampPriceInputModel>>();

            List<TimestampPriceInputModel> chunk = new List<TimestampPriceInputModel>();

            while (datasetOffset + windowSize <= dataset.Count)
            {
                chunk = dataset.GetRange(datasetOffset, windowSize)
                .ToList();

                datasetOffset++;

                if (windowSize != chunk.Count)
                {
                    break;
                }

                featuresGrouped.Add(chunk);
            }

            return featuresGrouped;
        }

        public static List<TabularModelInput> ToTabularDataInputModel
                                                (this List<List<TimestampPriceInputModel>> tabularDataset)
            => tabularDataset.Select(datapoint => new TabularModelInput()
            {
                Features = datapoint
                    .Select(e => e.Price)
                    .SkipLast(1)
                    .ToArray(),
                Label = datapoint
                    .Select(e => e.Price)
                    .Last(),
                FirstFeatureDate = datapoint.FirstOrDefault()?.Date ?? DateTime.MinValue
                
            }).ToList();
        
    }
}
