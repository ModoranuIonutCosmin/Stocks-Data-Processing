using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Services;

public interface ICompanyService
{
    public Company GetCompanyData(string ticker);
}