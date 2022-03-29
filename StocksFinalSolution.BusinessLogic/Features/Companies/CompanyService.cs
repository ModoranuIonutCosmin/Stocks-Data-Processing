using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Features.Companies;

public class CompanyService : ICompanyService
{
    private readonly ICompaniesRepository _companiesRepository;

    public CompanyService(ICompaniesRepository companiesRepository)
    {
        _companiesRepository = companiesRepository;
    }

    public Company GetCompanyData(string ticker)
    {
        return _companiesRepository.GetCompanyData(ticker);
    }
}