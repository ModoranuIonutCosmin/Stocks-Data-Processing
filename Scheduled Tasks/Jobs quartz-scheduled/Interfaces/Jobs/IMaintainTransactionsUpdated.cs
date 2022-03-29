using System.Threading.Tasks;
using Quartz;

namespace Stocks_Data_Processing.Interfaces.Jobs;

public interface IMaintainTransactionsUpdated : IJob
{
    Task UpdateTransactions();
}