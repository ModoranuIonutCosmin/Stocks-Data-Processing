using Quartz;
using Quartz.Impl.Triggers;
using Stocks.General.ConstantsConfig;
using Stocks_Data_Processing.Utilities;
using StocksProccesing.Relational.DataAccess;
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
        private readonly IScheduler _scheduler;
        private readonly StockContextFactory _dbContextFactory;


        public StocksDataHandlingLogic(IMaintainPredictionsUpToDate maintainPredictionsUpToDate,
            IMaintainTaxesCollected maintainTaxesCollected,
            IScheduler scheduler,
            StockContextFactory contextFactory
            )
        {
            _maintainPredictionsUpToDate = maintainPredictionsUpToDate;
            _maintainTaxesCollected = maintainTaxesCollected;
            _scheduler = scheduler;
            _dbContextFactory = contextFactory;
        }

        public async Task StartMantainingCurrentStocksData()
        {
            var currentStocksJob = JobBuilder.Create<MaintainCurrentStockData>()
                .WithIdentity(MaintananceJobsName.CurrentStocksValuesJob, "Maintenance").Build();

            //Fiecare minut intr-un workday in perioada de trade 8:00-23:59 UTC.
            var cronTrigger = new CronTriggerImpl();

            cronTrigger.CronExpression = new CronExpression("0 * 8-23 ? * MON,TUE,WED,THU,FRI *");
            cronTrigger.TimeZone = TimeZoneInfo.Utc;
            cronTrigger.JobName = MaintananceJobsName.CurrentStocksValuesJob;
            cronTrigger.JobGroup = "Maintenance";
            cronTrigger.Name = MaintananceJobsName.CurrentStocksValuesJob;
            cronTrigger.Group = "Maintenance";

            await _scheduler.ScheduleJob(currentStocksJob, cronTrigger);
        }

        public async Task StartPredictionEngine()
        {
            var predictionsJob = JobBuilder.Create<MaintainPredictionsUpToDate>().WithIdentity(MaintananceJobsName.PredictionsJob, "Maintenance").Build();

            //Fiecare sambata la 12:00 PM
            var predictionsTrigger = TriggerBuilder.Create()
                .WithIdentity(MaintananceJobsName.PredictionsJob, "Maintenance")
                .WithCronSchedule("0 0 12 ? * SAT")
                .StartNow().Build();
            await _scheduler.ScheduleJob(predictionsJob, predictionsTrigger);


            var dbContext = _dbContextFactory.Create();

            //Executa odata la pornire daca nu a mai fost executat vreodata si apoi fa schedule
            var maintananceActionsData = dbContext.Actions
                .Where(e => e.Type == MaintananceJobsName.PredictionsJob)
                .ToList();

            bool shouldRun = false;

            if (maintananceActionsData.Count == 0)
            {
                dbContext.Actions.Add(new MaintenanceAction()
                {
                    Type = MaintananceJobsName.PredictionsJob,

                });

                await dbContext.SaveChangesAsync();

                shouldRun = true;
            }
            else
            {
                var jobEntry = maintananceActionsData.Last();

                if (jobEntry.LastFinishedDate.Add(MaintenanceRoutinesPeriod.PredictionsJob) < DateTimeOffset.UtcNow)
                    shouldRun = true;
            }

            if (shouldRun)
            {
                await _maintainPredictionsUpToDate.UpdatePredictionsAsync();

                dbContext.Actions
                 .Where(e => e.Type == MaintananceJobsName.PredictionsJob)
                 .ToList().Last().LastFinishedDate = DateTimeOffset.UtcNow;

                await dbContext.SaveChangesAsync();
            }
        }



        public async Task StartTransactionsMonitoring()
        {
            var transactionClosingJob = JobBuilder.Create<MaintainTransactionsUpdated>()
                .WithIdentity(MaintananceJobsName.TransactionClosingJob, "Maintenance").Build();

            //Fiecare minut intr-un workday in perioada de trade 8:00-23:59 UTC.
            var cronTrigger = new CronTriggerImpl();

            cronTrigger.CronExpression = new CronExpression("0 * 8-23 ? * MON,TUE,WED,THU,FRI *");
            cronTrigger.TimeZone = TimeZoneInfo.Utc;
            cronTrigger.JobName = MaintananceJobsName.TransactionClosingJob;
            cronTrigger.JobGroup = "Maintenance";
            cronTrigger.Name = MaintananceJobsName.TransactionClosingJob;
            cronTrigger.Group = "Maintenance";

            await _scheduler.ScheduleJob(transactionClosingJob, cronTrigger);
        }

        public async Task StartTaxesCollecting()
        {
            

            var taxesCollectingJob = JobBuilder.Create<MaintainTaxesCollected>()
                .WithIdentity(MaintananceJobsName.TaxesCollectingJob, "Maintenance").Build();

            //Fiecare duminica la 12:00 PM
            var collectionTrigger = TriggerBuilder.Create()
                .WithIdentity(MaintananceJobsName.TaxesCollectingJob, "Maintenance")
                .WithCronSchedule("0 0 12 ? * SUN")
                .StartNow().Build();

            await _scheduler.ScheduleJob(taxesCollectingJob, collectionTrigger);

            var dbContext = _dbContextFactory.Create();

            //Executa odata la pornire daca nu a mai fost executat vreodata
            var maintananceActionsData = dbContext.Actions
                .Where(e => e.Type == MaintananceJobsName.TaxesCollectingJob)
                .ToList();

            bool shouldRun = false;

            if (maintananceActionsData.Count == 0)
            {
                dbContext.Actions.Add(new MaintenanceAction()
                {
                    Type = MaintananceJobsName.TaxesCollectingJob,

                });

                await dbContext.SaveChangesAsync();

                shouldRun = true;
            }
            else
            {
                var jobEntry = maintananceActionsData.Last();

                if (jobEntry.LastFinishedDate.Add(MaintenanceRoutinesPeriod.TaxesCollectingJob) < DateTimeOffset.UtcNow)
                    shouldRun = true;
            }

            if (shouldRun)
            {
                await _maintainTaxesCollected.CollectTaxes();

                dbContext.Actions
                 .Where(e => e.Type == MaintananceJobsName.TaxesCollectingJob)
                 .ToList().Last().LastFinishedDate = DateTimeOffset.UtcNow;

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task StartAllFunctions()
        {
            await Task.WhenAll(new List<Task> { StartMantainingCurrentStocksData(), 
                //StartPredictionEngine(),
                StartTaxesCollecting(), StartTransactionsMonitoring() });

            await _scheduler.Start();

            await Task.Delay(Timeout.InfiniteTimeSpan);
        }
    }
}
