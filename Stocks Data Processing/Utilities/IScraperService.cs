using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stocks_Data_Processing.Utilities
{
    public interface IScraperService
    {
        Task<decimal> GetNumericFieldValueByHtmlClassesCombination(string link, List<string> classes);
    }
}