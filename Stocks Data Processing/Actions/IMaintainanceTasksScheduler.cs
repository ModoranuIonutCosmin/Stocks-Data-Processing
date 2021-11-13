using Quartz;
using StocksProccesing.Relational.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stocks_Data_Processing.Actions
{
    public interface IMaintainanceTasksScheduler
    {
        Task<List<Task>> ScheduleJobs(List<MaintenanceAction> jobs);
    }
}