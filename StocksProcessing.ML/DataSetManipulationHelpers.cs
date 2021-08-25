using StocksProcessing.ML.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StocksProcessing.ML
{
    public static class DataSetManipulationHelpers
    {
        public static SeparatedPricesDataInputModel SeparateDataSet 
            (this IEnumerable<PriceDataInputModel> allData, double testFraction)

        {
            double trainFraction = 1 - testFraction;

            IEnumerable<PriceDataInputModel> trainList 
                = allData.Take((int)(allData.Count() * trainFraction));
            IEnumerable<PriceDataInputModel> testList 
                = allData.Skip(Math.Max(0, (int)(allData.Count() * trainFraction)));

            return new SeparatedPricesDataInputModel()
            {
                TrainData = trainList,
                TestData = testList
            };
        }
    }
}
