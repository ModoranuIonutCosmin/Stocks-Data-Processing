using System.Collections.Generic;
using System.Threading.Tasks;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories.Base;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Repositories;

public interface IMaintainanceJobsRepository : IRepository<MaintenanceAction, int>
{
    Task<List<MaintenanceAction>> EnsureJobsDataExists(List<MaintenanceAction> jobsData);
    MaintenanceAction GetMaintenanceActionByName(string name);
    void MarkJobFinished(string jobName);
}