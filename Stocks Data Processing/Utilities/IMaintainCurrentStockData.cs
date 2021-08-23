using System.Threading.Tasks;

namespace Stocks_Data_Processing.Utilities
{
    /// <summary>
    /// Interfata unui serviciu ce isi propune sa updateze in baza de date
    /// datele obtinute printr-un serviciu ce implementeaza 
    /// interfata <see cref="ICurrentStockInfoDataScraperService"/>
    /// </summary>
    public interface IMaintainCurrentStockData
    {
        Task UpdateStockDataAsync();
    }
}