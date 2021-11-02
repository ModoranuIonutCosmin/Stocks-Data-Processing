using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Model;
using System.Collections.Generic;
using System.Linq;

namespace StocksProccesing.Relational.Repositories
{
    public class CompaniesRepository : ICompaniesRepository
    {
        public StocksMarketContext _dbContext { get; set; }

        public CompaniesRepository(StocksMarketContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Company GetCompanyData(string ticker)
        {
            return _dbContext.Companies.FirstOrDefault(c => c.Ticker == ticker);
        }

        public List<Company> GetAllStocksCompanies()
        {
            return _dbContext.Companies.ToList();
        }
    }
}
