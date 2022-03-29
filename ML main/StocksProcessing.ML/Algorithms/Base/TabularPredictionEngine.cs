﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML.Data;
using StocksProcessing.ML.Models.Tabular;

namespace StocksProcessing.ML.Algorithms.Base;

public abstract class TabularPredictionEngine : PredictionEngineBase<TabularModelInput, TabularModelOutput>
{
    public int WindowSize => _dataset.FirstOrDefault()?.GetLineSize() ?? 0;
    protected TabularPredictionEngine(IEnumerable<TabularModelInput> dataset) : base(dataset)
    {
        
    }

    public override SchemaDefinition CreateCustomSchemaDefinition()
    {
        int featuresDimension = WindowSize;

        SchemaDefinition autoSchema = SchemaDefinition.Create(typeof(TabularModelInput));
        
        SchemaDefinition.Column featureColumn = autoSchema[0];
        PrimitiveDataViewType itemType = ((VectorDataViewType)featureColumn.ColumnType).ItemType;
        featureColumn.ColumnType = new VectorDataViewType(itemType, featuresDimension);
        featureColumn.ColumnName = "Features";
        
        SchemaDefinition.Column labelColumn = autoSchema[1];
        labelColumn.ColumnName = "Label";
        

        return autoSchema;
    }

    public override async Task<List<PredictionResult>> ComputePredictionsForNextPeriod(int horizon,
        double testFraction)
    {
        if (PredictionEngine == null)
        {
            await TrainModel(horizon, testFraction);
        }

        if (PredictionEngine == null)
        {
            throw new Exception("Failed model training - got nothing");
        }

        var result = new List<PredictionResult>();
        TabularModelInput previousDataEntry = _dataset.Last();

        previousDataEntry.Label = 0;

        for (int predictionIndex = 0; predictionIndex < horizon; predictionIndex++)
        {
            var nextPrice = this.PredictionEngine.Predict(previousDataEntry);

            float[] nextFeatures = new float[previousDataEntry.Features.Length];
            Array.Copy(previousDataEntry.Features, 1, nextFeatures,
                0, previousDataEntry.Features.Length - 1);
            nextFeatures[^1] = nextPrice.Score;
            previousDataEntry.Features = nextFeatures;

            result.Add(new PredictionResult()
            {
                Price = nextPrice.Score
            });
        }

        return result;
    }
}