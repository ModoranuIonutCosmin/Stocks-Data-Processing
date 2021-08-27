using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl.Triggers;
using Stocks_Data_Processing.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Stocks_Data_Processing
{
    public class StocksDataHandlingLogic : IStocksDataHandlingLogic
    {
        private readonly ILogger _logger;
        private readonly IMaintainCurrentStockData _maintainerCurrentStockService;
        private readonly IMaintainPredictionsUpToDate _maintainPredictionsUpToDateService;
        private readonly IScheduler _scheduler;

        public StocksDataHandlingLogic(ILogger<StocksDataHandlingLogic> logger,
            IMaintainCurrentStockData maintainerCurrentStockService,
            IMaintainPredictionsUpToDate maintainPredictionsUpToDateService,
            IScheduler scheduler
            )
        {
            _logger = logger;
            _maintainerCurrentStockService = maintainerCurrentStockService;
            _maintainPredictionsUpToDateService = maintainPredictionsUpToDateService;
            _scheduler = scheduler;
        }

        public async Task StartMantainingCurrentStocksData()
        {
            var currentStocksJob = JobBuilder.Create<MaintainCurrentStockData>().WithIdentity("CurrentStock", "Maintenance").Build();

            //Fiecare minut intr-un workday in perioada de trade 8:00-23:59 UTC.
            var cronTrigger = new CronTriggerImpl();

            cronTrigger.CronExpression = new CronExpression("0 8-23 * ? * MON-FRI");
            cronTrigger.TimeZone = TimeZoneInfo.Utc;
            cronTrigger.JobName = "CurrentStock";
            cronTrigger.JobGroup = "Maintenance";
            cronTrigger.Name = "CurrentStock";
            cronTrigger.Group = "Maintenance";

            await _scheduler.ScheduleJob(currentStocksJob, cronTrigger);
        }

        public async Task StartPredictionEngine()
        {
            var predictionsJob = JobBuilder.Create<MaintainPredictionsUpToDate>().WithIdentity("Predictions", "Maintenance").Build();

            //Fiecare sambata la 12:00 PM
            var predictionsTrigger = TriggerBuilder.Create()
                .WithIdentity("Predictions", "Maintenance")
                .StartNow()
                .WithCronSchedule("0 0 12 ? * SAT").Build();
            await _scheduler.ScheduleJob(predictionsJob, predictionsTrigger);
        }

        public async Task StartAllFunctions()
        {
            await Task.WhenAll(new List<Task> { StartMantainingCurrentStocksData(), StartPredictionEngine() });

            await _scheduler.Start();

            await Task.Delay(Timeout.InfiniteTimeSpan);
        }
    }
}
