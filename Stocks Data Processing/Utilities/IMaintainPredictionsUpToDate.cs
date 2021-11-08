using Quartz;
using System.Threading.Tasks;

namespace Stocks_Data_Processing.Utilities
{
    public interface IMaintainPredictionsUpToDate : IJob
    {
        Task UpdatePredictionsAsync();
    }
}