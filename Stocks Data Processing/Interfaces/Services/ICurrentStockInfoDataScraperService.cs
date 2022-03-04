using System.Collections.Generic;
using System.Threading.Tasks;
using Stocks_Data_Processing.Models;

namespace Stocks_Data_Processing.Interfaces.Services
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