using System.Threading.Tasks;
using Quartz;

namespace Stocks_Data_Processing.Interfaces.Jobs
{
    public interface IMaintainPredictionsUpToDate : IJob
    {
        Task UpdatePredictionsAsync();
    }
}