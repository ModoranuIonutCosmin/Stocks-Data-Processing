using Microsoft.Extensions.Logging;
using Stocks_Data_Processing.Models;
using Stocks_Data_Processing.Utilities;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Model;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Stocks_Data_Processing
{
    public class StocksDataHandlingLogic : IStocksDataHandlingLogic
    {
        private readonly ILogger _logger;
        private readonly IMaintainCurrentStockData _maintainerCurrentStockService;
        private readonly IMaintainPredictionsUpToDate _maintainPredictionsUpToDateService;

        public StocksDataHandlingLogic(ILogger<StocksDataHandlingLogic> logger,
            IMaintainCurrentStockData maintainerCurrentStockService,
            IMaintainPredictionsUpToDate maintainPredictionsUpToDateService
            )
        {
            _logger = logger;
            _maintainerCurrentStockService = maintainerCurrentStockService;
            _maintainPredictionsUpToDateService = maintainPredictionsUpToDateService;
        }

        public async Task StartMantainingCurrentStocksData()
        {

            //while (true)
            //{
            //    await _maintainerCurrentStockService.UpdateStockDataAsync();

            //    _logger.LogWarning("Updated stocks!");

                await Task.Delay(TimeSpan.FromMinutes(1));
            //}
        }

        public async Task StartPredictionEngine()
        {
            _logger.LogWarning("Started to maintain predictions up-to-date!");

            await _maintainPredictionsUpToDateService.UpdatePredictionsAsync();
        }
    }
}
