using System;
using System.Threading.Tasks;
using Quartz;

namespace Stocks_Data_Processing.Interfaces.Jobs
{
    public interface IMaintainPeriodicalSummaries : IJob
    {
        Task UpdateLastPeriod(string ticker, TimeSpan period);
    }
}