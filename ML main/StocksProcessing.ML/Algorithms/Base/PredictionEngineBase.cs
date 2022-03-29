using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Stocks.General.ExtensionMethods;
using StocksProcessing.ML.Models;
using StocksProcessing.ML.Models.Tabular;

namespace StocksProcessing.ML.Algorithms.Base;

public abstract class PredictionEngineBase<TMI, TMO> : IPredictionEngine
    where TMI : class, IInputModel, new()
    where TMO: class, new()
{
    protected IEnumerable<TMI> _dataset;
    protected IDataView TrainData { get; set; }
    protected IDataView TestData { get; set; }
    protected MLContext MlContext { get; set; }
    protected dynamic TrainPipeline { get; set; }
    
    protected Microsoft.ML.PredictionEngineBase<TMI, TMO> PredictionEngine;
    
    public PredictionEngineBase(IEnumerable<TMI> dataset)
    {
        _dataset = dataset;
        MlContext = new MLContext();
    }

    public abstract Task SetupPipeline(int horizon);
    public virtual async Task CreatePredictionEngine(ITransformer model)
    {
        PredictionEngine = MlContext.Model.CreatePredictionEngine<TMI, TMO>(model,
            inputSchemaDefinition: CreateCustomSchemaDefinition());
    }
    public virtual async Task TrainModel(int horizon, double testFraction)
    {
        SeparatedDataset<TMI> separatedDataset = _dataset.SeparateDataSet(testFraction);

        var customSchema = CreateCustomSchemaDefinition();
        
        TrainData = MlContext.Data.LoadFromEnumerable(separatedDataset.TrainData, customSchema);
        TestData = MlContext.Data.LoadFromEnumerable(separatedDataset.TestData, customSchema);
        dynamic model;
        await SetupPipeline(horizon);
        try
        {
            model = TrainPipeline.Fit(TrainData);
            CreatePredictionEngine(model);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public abstract SchemaDefinition CreateCustomSchemaDefinition();
    public abstract Task<List<PredictionResult>> ComputePredictionsForNextPeriod(int horizon,
        double testFraction);
    public virtual async Task<(AccuracyStatistics accuracy, List<PredictionResult> predictions)> EvaluateModel
        (int horizon, double testFraction, TimeSpan interval)
    {
        var forecastedPrices = await ComputePredictionsForNextPeriod(horizon, testFraction);

        var testData = MlContext.Data
            .CreateEnumerable<TMI>(TestData, reuseRowObject: false)
            .ToList();

        var testDataPriceObservations
            = testData.Select(data => data.GetLabel());

        var currentForecastedEntry = testData.FirstOrDefault()
            ?.GetObservationDate() ?? DateTimeOffset.MinValue;
        
        forecastedPrices.ForEach(
            prediction =>
            {
                currentForecastedEntry = currentForecastedEntry.GetNextStockMarketTime(interval);
                prediction.Date = currentForecastedEntry;
            });

        return (new AccuracyStatistics() 
        {
            MAE = forecastedPrices.Select( pred => pred.Price).CalculateMAE(testDataPriceObservations),
            RMSE = forecastedPrices.Select( pred => pred.Price).CalculateRMSE(testDataPriceObservations),
        }, forecastedPrices);
    }
}