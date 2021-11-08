using Quartz;
using Quartz.Impl.Triggers;
using Stocks.General.ConstantsConfig;
using Stocks_Data_Processing.Utilities;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.DataAccess.V1.Repositories;
using StocksProccesing.Relational.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Stocks_Data_Processing
{
    public class StocksDataHandlingLogic : IStocksDataHandlingLogic
    {

        private readonly IMaintainPredictionsUpToDate _maintainPredictionsUpToDate;
        private readonly IMaintainTaxesCollected _maintainTaxesCollected;
        private readonly IMaintenanceJobsRepository _maintenanceJobsRepository;
        private readonly IMaintainCurrentStockData _maintainCurrentStockData;

        public StocksDataHandlingLogic(IMaintainPredictionsUpToDate maintainPredictionsUpToDate,
            IMaintainTaxesCollected maintainTaxesCollected,
            IMaintenanceJobsRepository maintenanceJobsRepository,
            IMaintainCurrentStockData maintainCurrentStockData
            )
        {
            _maintainPredictionsUpToDate = maintainPredictionsUpToDate;
            _maintainTaxesCollected = maintainTaxesCollected;
            _maintenanceJobsRepository = maintenanceJobsRepository;
            _maintainCurrentStockData = maintainCurrentStockData;
        }

        public async Task StartAllFunctions()
        {
            await Task.WhenAll(new List<Task> { _maintainCurrentStockData.Execute(), 
                //StartPredictionEngine(),
                StartTaxesCollecting(), StartTransactionsMonitoring() });

            await Task.Delay(Timeout.InfiniteTimeSpan);
        }
    }
}
