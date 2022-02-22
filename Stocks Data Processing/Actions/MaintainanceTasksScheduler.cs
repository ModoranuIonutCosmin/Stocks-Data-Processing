using Quartz;
using Stocks_Data_Processing.Quartz_Helpers;
using Stocks_Data_Processing.Utilities;
using StocksProccesing.Relational.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stocks_Data_Processing.Actions
{
    public class MaintainanceTasksScheduler : IMaintainanceTasksScheduler
    {
        private readonly IScheduler scheduler;
        private readonly Dictionary<string, IJob> PeriodicalTasks = new();

        private readonly Dictionary<string, IJob> AllTasks = new();


        public MaintainanceTasksScheduler(IScheduler scheduler)
        {
            PeriodicalTasks.Add(MaintainanceTasksSchedulerHelpers.TaxesCollectJob, DIContainerConfig.Resolve<IMaintainTaxesCollected>());
            PeriodicalTasks.Add(MaintainanceTasksSchedulerHelpers.PredictionsRefreshJob, DIContainerConfig.Resolve<IMaintainPredictionsUpToDate>());

            AllTasks.Add(MaintainanceTasksSchedulerHelpers.TaxesCollectJob, DIContainerConfig.Resolve<IMaintainTaxesCollected>());
            AllTasks.Add(MaintainanceTasksSchedulerHelpers.PredictionsRefreshJob, DIContainerConfig.Resolve<IMaintainPredictionsUpToDate>());
            AllTasks.Add(MaintainanceTasksSchedulerHelpers.CurrentSummariesJob, DIContainerConfig.Resolve<IMaintainPeriodicalSummaries>());
            AllTasks.Add(MaintainanceTasksSchedulerHelpers.CurrentStocksJob, DIContainerConfig.Resolve<IMaintainCurrentStockData>());
            AllTasks.Add(MaintainanceTasksSchedulerHelpers.TransactionMonitorJob, DIContainerConfig.Resolve<IMaintainTransactionsUpdated>());

            this.scheduler = scheduler;
        }

        public async Task<List<Task>> ScheduleJobs(List<MaintenanceAction> jobs)
        {
            var pendingTasks = new List<Task>();

            foreach (var job in jobs)
            {
                var implementation = AllTasks[job.Name];

                if (PeriodicalTasks.ContainsKey(job.Name) &&
                    job.LastFinishedDate.AddTicks(job.Interval) < DateTimeOffset.UtcNow)
                {
                    pendingTasks.Add(implementation.Execute(default));
                }

                await scheduler.ScheduleJobAsync(implementation, job);
            }

            await scheduler.Start();

            return pendingTasks;
        }
    }
}
