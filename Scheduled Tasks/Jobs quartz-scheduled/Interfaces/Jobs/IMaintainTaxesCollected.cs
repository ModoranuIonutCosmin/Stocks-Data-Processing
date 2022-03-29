using System.Threading.Tasks;
using Quartz;

namespace Stocks_Data_Processing.Interfaces.Jobs;

public interface IMaintainTaxesCollected : IJob
{
    Task CollectTaxes();
}