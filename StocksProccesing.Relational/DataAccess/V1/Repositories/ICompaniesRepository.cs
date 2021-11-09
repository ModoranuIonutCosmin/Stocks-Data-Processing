﻿using StocksProccesing.Relational.Interfaces;
using StocksProccesing.Relational.Model;
using System.Collections.Generic;

namespace StocksProccesing.Relational.DataAccess.V1.Repositories
{
    public interface ICompaniesRepository : IRepository<Company, int>
    {
        void EnsureCompaniesDataExists();
        List<Company> GetAllStocksCompanies();
        public Company GetCompanyData(string ticker);
    }
}