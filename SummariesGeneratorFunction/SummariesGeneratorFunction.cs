using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Quartz;
using Stocks_Data_Processing.Interfaces.Jobs;
using System.Threading.Tasks;

namespace MaintainanceStockProcessingJobs
{
    public class SummariesGeneratorFunction
    {
        private readonly IMaintainPeriodicalSummaries maintainPeriodicalSummaries;

        public SummariesGeneratorFunction(IMaintainPeriodicalSummaries maintainPeriodicalSummaries)
        {
            this.maintainPeriodicalSummaries = maintainPeriodicalSummaries;
        }

        [FunctionName("SummariesGeneratorFunction")]
        public async Task Run([TimerTrigger("0 */5 * * * *")]
        TimerInfo myTimer, ILogger log)
        {
            await maintainPeriodicalSummaries.Execute(default);
        }
    }
}
