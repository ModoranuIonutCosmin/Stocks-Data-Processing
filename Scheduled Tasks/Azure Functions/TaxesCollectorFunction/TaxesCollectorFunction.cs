using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Stocks_Data_Processing.Interfaces.Jobs;

namespace MaintainanceStockProcessingJobs;

public class TaxesCollectorFunction
{
    private readonly IMaintainTaxesCollected maintainTaxesCollected;

    public TaxesCollectorFunction(IMaintainTaxesCollected maintainTaxesCollected)
    {
        this.maintainTaxesCollected = maintainTaxesCollected;
    }

    [FunctionName("TaxesCollectorFunction")]
    public async Task Run([TimerTrigger("0 0 16 * * SAT")] TimerInfo myTimer, ILogger log)
    {
        await maintainTaxesCollected.Execute(default);
    }
}