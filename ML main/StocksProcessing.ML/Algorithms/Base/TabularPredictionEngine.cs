using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML.Data;
using StocksProcessing.ML.Models.Tabular;

namespace StocksProcessing.ML.Algorithms.Base;

public abstract class TabularPredictionEngine : PredictionEngineBase<TabularModelInput, TabularModelOutput>
{
    protected TabularPredictionEngine(IEnumerable<TabularModelInput> dataset) : base(dataset)
    {
    }

    public int WindowSize => _dataset.FirstOrDefault()?.GetLineSize() ?? 0;

    public override SchemaDefinition CreateCustomSchemaDefinition()
    {
        var featuresDimension = WindowSize;

        var autoSchema = SchemaDefinition.Create(typeof(TabularModelInput));

        var featureColumn = autoSchema[0];
        var itemType = ((VectorDataViewType) featureColumn.ColumnType).ItemType;
        featureColumn.ColumnType = new VectorDataViewType(itemType, featuresDimension);
        featureColumn.ColumnName = "Features";

        var labelColumn = autoSchema[1];
        labelColumn.ColumnName = "Label";


        return autoSchema;
    }

    public override async Task<List<PredictionResult>> ComputePredictionsForNextPeriod(int horizon,
        double testFraction)
    {
        if (_theirPredictionEngine == null) await TrainModel(horizon, testFraction);

        if (_theirPredictionEngine == null) throw new Exception("Failed model training - got nothing");

        var result = new List<PredictionResult>();
        var previousDataEntry = _dataset.Last();

        previousDataEntry.Label = 0;

        for (var predictionIndex = 0; predictionIndex < horizon; predictionIndex++)
        {
            var nextPrice = _theirPredictionEngine.Predict(previousDataEntry);

            var nextFeatures = new float[previousDataEntry.Features.Length];
            Array.Copy(previousDataEntry.Features, 1, nextFeatures,
                0, previousDataEntry.Features.Length - 1);
            nextFeatures[^1] = nextPrice.Score;
            previousDataEntry.Features = nextFeatures;

            result.Add(new PredictionResult
            {
                Price = nextPrice.Score
            });
        }

        return result;
    }
}