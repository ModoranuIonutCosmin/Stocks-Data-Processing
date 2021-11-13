using StocksProccesing.Relational.Interfaces;
using StocksProccesing.Relational.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StocksProccesing.Relational.DataAccess.V1.Repositories
{
    public interface IMaintainanceJobsRepository : IRepository<MaintenanceAction, int>
    {
        Task<List<MaintenanceAction>> EnsureJobsDataExists(List<MaintenanceAction> jobsData);
        MaintenanceAction GetMaintenanceActionByName(string name);
        void MarkJobFinished(string jobName);
    }
}