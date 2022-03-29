using System.Collections.Generic;
using System.Threading.Tasks;
using StocksProccesing.Relational.Model;

namespace Stocks_Data_Processing.Actions;

public interface IMaintainanceTasksScheduler
{
    Task<List<Task>> ScheduleJobs(List<MaintenanceAction> jobs);
}