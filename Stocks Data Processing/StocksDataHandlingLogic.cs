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
        private readonly IMaintainCurrentStockData _maintainCurrentStockData;
        private readonly IMaintainPeriodicalSummaries maintainPeriodicalSummaries;

        public StocksDataHandlingLogic(IMaintainPredictionsUpToDate maintainPredictionsUpToDate,
            IMaintainTaxesCollected maintainTaxesCollected,
            IMaintainCurrentStockData maintainCurrentStockData,
            IMaintainPeriodicalSummaries maintainPeriodicalSummaries
            )
        {
            _maintainPredictionsUpToDate = maintainPredictionsUpToDate;
            _maintainTaxesCollected = maintainTaxesCollected;
            _maintainCurrentStockData = maintainCurrentStockData;
            this.maintainPeriodicalSummaries = maintainPeriodicalSummaries;
        }

        public async Task StartAllFunctions()
        {
            await Task.WhenAll(new List<Task> { maintainPeriodicalSummaries.Execute(default),
                //_maintainCurrentStockData.Execute(default)
            });

            await Task.Delay(Timeout.InfiniteTimeSpan);
        }
    }
}
