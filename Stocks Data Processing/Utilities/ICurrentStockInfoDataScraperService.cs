using Stocks_Data_Processing.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stocks_Data_Processing.Utilities
{
    /// <summary>
    /// Interfata cu metode ce isi propun obtinerea datelor din prezent
    /// referitoare la valoarea stock-urilor unei companii 
    /// sau a unei liste de companii.
    /// </summary>
    public interface ICurrentStockInfoDataScraperService
    {
        Task<StockCurrentInfoResponse> GatherAsync(string ticker);
        Task<IList<StockCurrentInfoResponse>> GatherAllAsync();
    }
}