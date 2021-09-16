using Quartz;
using System.Threading.Tasks;

namespace Stocks_Data_Processing.Utilities
{
    public interface IMaintainTransactionsUpdated
    {
        Task UpdateTransactions();
    }
}