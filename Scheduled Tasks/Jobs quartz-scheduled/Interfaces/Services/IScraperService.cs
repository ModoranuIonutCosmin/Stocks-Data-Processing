using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stocks_Data_Processing.Interfaces.Services;

public interface IScraperService
{
    Task<List<decimal>> ExtractNumericFields(string url, string xPath);
}