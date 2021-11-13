using Stocks_Data_Processing.Actions;
using Stocks_Data_Processing.ConfigHelpers;
using Stocks_Data_Processing.Utilities;
using StocksProccesing.Relational.DataAccess.V1.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Stocks_Data_Processing
{
    public class StocksDataHandlingLogic : IStocksDataHandlingLogic
    {

        private readonly IMaintainanceTasksScheduler maintainanceTasksScheduler;
        private readonly IMaintainanceJobsRepository maintenanceJobsRepository;

        public StocksDataHandlingLogic(
            IMaintainanceTasksScheduler maintainanceTasksScheduler,
            IMaintainanceJobsRepository maintenanceJobsRepository
            )
        {
            this.maintainanceTasksScheduler = maintainanceTasksScheduler;
            this.maintenanceJobsRepository = maintenanceJobsRepository;
        }

        public async Task InitializeTasks()
        {
            var tasksToBeScheduled = ReadTasksConfigHelpers
                .GetMaintainanceActions(MaintainanceTasksSchedulerHelpers.allTasksNames);
            var tasksInDatabase = await maintenanceJobsRepository.EnsureJobsDataExists(tasksToBeScheduled);
            var pendingTasks = await maintainanceTasksScheduler.ScheduleJobs(tasksInDatabase);

            await Task.WhenAll(pendingTasks);
        }

        public async Task StartAllFunctions()
        {
            await InitializeTasks();
            await Task.Delay(Timeout.Infinite);
        }
    }
}
