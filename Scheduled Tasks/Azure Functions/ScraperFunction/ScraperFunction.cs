using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Stocks_Data_Processing.Interfaces.Jobs;

namespace MaintananceJobs
{
    public class ScraperFunction
    {
        private readonly IMaintainCurrentStockData maintainCurrentStockData;

        public ScraperFunction(IMaintainCurrentStockData maintainCurrentStockData)
        {
            this.maintainCurrentStockData = maintainCurrentStockData;
        }

        [FunctionName("ScraperFunction")]
        public async Task Run([TimerTrigger("0 * 8-23 * * 1-5")]TimerInfo myTimer, ILogger log)
        {
            await maintainCurrentStockData.Execute(default);
        }
    }
}
