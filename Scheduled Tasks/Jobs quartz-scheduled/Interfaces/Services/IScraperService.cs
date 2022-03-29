using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stocks_Data_Processing.Interfaces.Services
{
    public interface IScraperService
    {
        Task<decimal> GetNumericFieldValueByHtmlClassesCombination(string link, List<string> classes);
    }
}