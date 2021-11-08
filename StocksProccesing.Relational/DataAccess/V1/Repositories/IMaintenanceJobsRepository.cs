using StocksProccesing.Relational.Interfaces;
using StocksProccesing.Relational.Model;
using System.Threading.Tasks;

namespace StocksProccesing.Relational.DataAccess.V1.Repositories
{
    public interface IMaintenanceJobsRepository : IRepository<MaintenanceAction, int>
    {
        Task<MaintenanceAction> GetByTypeAsync(string type);
    }
}