using System;
using System.Collections.Generic;
using System.Linq;
using StocksProcessing.ML.Models;
using StocksProcessing.ML.Models.Tabular;
using StocksProcessing.ML.Models.TimeSeries;

namespace StocksProcessing.ML;

public static class DataSetManipulationHelpers
{
    public static SeparatedDataset<TM> SeparateDataSet<TM>
        (this IEnumerable<TM> allData, double testFraction)

    {
        var trainFraction = 1 - testFraction;

        var trainList
            = allData.Take((int) (allData.Count() * trainFraction));
        var testList
            = allData.Skip(Math.Max(0, (int) (allData.Count() * trainFraction)));

        return new SeparatedDataset<TM>
        {
            TrainData = trainList,
            TestData = testList
        };
    }

    public static List<List<TimestampPriceInputModel>> Tabularize(this IEnumerable<TimestampPriceInputModel> allData,
        int windowSize)
    {
        var datasetOffset = 0;
        var dataset = new List<TimestampPriceInputModel>(allData);
        var featuresGrouped = new List<List<TimestampPriceInputModel>>();

        var chunk = new List<TimestampPriceInputModel>();

        while (datasetOffset + windowSize <= dataset.Count)
        {
            chunk = dataset.GetRange(datasetOffset, windowSize)
                .ToList();

            datasetOffset++;
            
            if (windowSize != chunk.Count) break;

            featuresGrouped.Add(chunk);
        }

        return featuresGrouped;
    }

    public static List<TabularModelInput> ToTabularDataInputModel
        (this List<List<TimestampPriceInputModel>> tabularDataset)
    {
        return tabularDataset.Select(datapoint => new TabularModelInput
        {
            Features = datapoint
                .Select(e => e.Price)
                .SkipLast(1)
                .ToArray(),
            Label = datapoint
                .Select(e => e.Price)
                .Last(),
            LastFeatureDate = datapoint.LastOrDefault()?.Date ?? DateTime.MinValue
        }).ToList();
    }
}