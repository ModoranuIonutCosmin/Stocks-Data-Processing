using System.Collections.Generic;
using System.Threading.Tasks;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories.Base;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Repositories
{
    public interface ICompaniesRepository : IRepository<Company, int>
    {
        void EnsureCompaniesDataExists();
        List<Company> GetAllStocksCompanies();
        public Company GetCompanyData(string ticker);
        Task<Company> GetPredictionsByTicker(string ticker);
    }
}