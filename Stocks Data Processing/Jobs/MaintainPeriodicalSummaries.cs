using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Stocks.General.ExtensionMethods;
using Stocks_Data_Processing.Actions;
using Stocks_Data_Processing.Interfaces.Jobs;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksFinalSolution.BusinessLogic.StocksMarketSummaryGenerator;
using StocksProccesing.Relational.Model;

namespace Stocks_Data_Processing.Jobs
{
    public class MaintainPeriodicalSummaries : IMaintainPeriodicalSummaries
    {
        private readonly IStocksSummaryGenerator stocksSummaryGenerator;
        private readonly IStockSummariesRepository stockSummariesRepository;
        private readonly IMaintainanceJobsRepository jobsRepository;

        private List<TimeSpan> periods { get; set; } = new List<TimeSpan>{ TimeSpan.FromMinutes(5), TimeSpan.FromDays(1),
            TimeSpan.FromDays(7) };
       
        public MaintainPeriodicalSummaries(
            IStocksSummaryGenerator stocksSummaryGenerator,
            IStockSummariesRepository stockSummariesRepository,
            IMaintainanceJobsRepository jobsRepository)
        {
            this.stocksSummaryGenerator = stocksSummaryGenerator;
            this.stockSummariesRepository = stockSummariesRepository;
            this.jobsRepository = jobsRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var tickersList = TickersHelpers.GatherAllTickers();

            foreach(var ticker in tickersList)
            {
                foreach(var period in periods)
                    await UpdateLastPeriod(ticker, period);
            }
        }

        public async Task UpdateLastPeriod(string ticker, TimeSpan period)
        {
            var startingRange = DateTime.UtcNow.Subtract(period);

            //Sterge ultimul range
            await stockSummariesRepository
                .DeleteWhereAsync(e => e.CompanyTicker == ticker &&
                e.Date >= startingRange && e.Period == period.Ticks);

            var lastRangeSummary = await stocksSummaryGenerator.GenerateSummary(ticker, period);

            await stockSummariesRepository
                .AddRangeAsync(lastRangeSummary.Timepoints
                .Select(k => new StocksOhlc()
                {
                    CloseValue = k.CloseValue,
                    CompanyTicker = ticker,
                    Date = k.Date,
                    High = k.High,
                    Low = k.Low,
                    OpenValue = k.OpenValue,
                    Period = period.Ticks
                }).ToList());

            jobsRepository.MarkJobFinished(MaintainanceTasksSchedulerHelpers.CurrentSummariesJob);
        }
    }
}
