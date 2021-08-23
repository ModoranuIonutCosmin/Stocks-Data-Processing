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
        private readonly StocksMarketContext _dbContext;

        public StocksDataHandlingLogic(ILogger<StocksDataHandlingLogic> logger,
            IMaintainCurrentStockData maintainerCurrentStockService,
            StocksMarketContext dbContext)
        {
            _logger = logger;
            _maintainerCurrentStockService = maintainerCurrentStockService;
            _dbContext = dbContext;
        }

        public async Task StartMantainingCurrentStocksData()
        {
            Console.WriteLine("Started to maintain current stocks data!");

            while (true)
            {
                await _maintainerCurrentStockService.UpdateStockDataAsync();

                _logger.LogInformation("Updated stocks!");

                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }

        public async Task StartPredictionEngine()
        {
            System.Console.WriteLine("Started prediction engine!");
        }
    }
}
