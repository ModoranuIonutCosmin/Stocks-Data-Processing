using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Stocks_Data_Processing.Interfaces.Jobs;
using System.Threading.Tasks;

namespace MaintainanceStockProcessingJobs
{
    public class TransactionsMonitorFunction
    {
        private readonly IMaintainTransactionsUpdated maintainTransactionsUpdated;

        public TransactionsMonitorFunction(IMaintainTransactionsUpdated maintainTransactionsUpdated)
        {
            this.maintainTransactionsUpdated = maintainTransactionsUpdated;
        }

        [FunctionName("TransactionsMonitorFunction")]
        public async Task Run([TimerTrigger("0 * 8-23 * * *")] TimerInfo myTimer, ILogger log)
        {
            await maintainTransactionsUpdated.Execute(default);
        }
    }
}
