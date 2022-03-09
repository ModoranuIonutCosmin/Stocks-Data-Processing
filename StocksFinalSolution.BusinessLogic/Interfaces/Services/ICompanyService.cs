using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Features.Companies;

public interface ICompanyService
{
    public Company GetCompanyData(string ticker);
}