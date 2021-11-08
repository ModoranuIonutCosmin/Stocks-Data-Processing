using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Model;
using System.Collections.Generic;

namespace StocksProccesing.Relational.Repositories
{
    public interface ICompaniesRepository : IEFRepository<StocksMarketContext>
    {
        void EnsureCompaniesDataExists();
        List<Company> GetAllStocksCompanies();
        public Company GetCompanyData(string ticker);
    }
}