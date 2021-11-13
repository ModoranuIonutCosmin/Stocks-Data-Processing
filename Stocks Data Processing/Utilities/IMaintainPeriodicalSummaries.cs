using Quartz;
using System;
using System.Threading.Tasks;

namespace Stocks_Data_Processing.Utilities
{
    public interface IMaintainPeriodicalSummaries : IJob
    {
        Task UpdateLastPeriod(string ticker, TimeSpan period);
    }
}