using System.Threading.Tasks;
using Quartz;
using Stocks_Data_Processing.Interfaces.Services;

namespace Stocks_Data_Processing.Interfaces.Jobs
{
    /// <summary>
    /// Interfata unui serviciu ce isi propune sa updateze in baza de date
    /// datele obtinute printr-un serviciu ce implementeaza 
    /// interfata <see cref="ICurrentStockInfoDataScraperService"/>
    /// </summary>
    public interface IMaintainCurrentStockData : IJob
    {
        Task UpdateStockDataAsync();
    }
}