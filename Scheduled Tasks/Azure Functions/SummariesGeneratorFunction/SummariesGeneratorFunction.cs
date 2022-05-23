using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Stocks_Data_Processing.Interfaces.Jobs;

namespace SummariesGeneratorFunction;

public class SummariesGeneratorFunction
{
    private readonly IMaintainPeriodicalSummaries maintainPeriodicalSummaries;

    public SummariesGeneratorFunction(IMaintainPeriodicalSummaries maintainPeriodicalSummaries)
    {
        this.maintainPeriodicalSummaries = maintainPeriodicalSummaries;
    }

    [FunctionName("SummariesGeneratorFunction")]
    public async Task Run([TimerTrigger("0 */5 * * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
    {
        await maintainPeriodicalSummaries.Execute(default);
    }
}