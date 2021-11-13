using Microsoft.Extensions.Logging;
using Quartz;
using Stocks_Data_Processing.Actions;
using StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.DataAccess.V1.Repositories;
using System;
using System.Threading.Tasks;

namespace Stocks_Data_Processing.Utilities
{
    public class MaintainTransactionsUpdated : IMaintainTransactionsUpdated
    {
        private readonly ILogger<MaintainTransactionsUpdated> _logger;
        private readonly IStockMarketProfitCalculator profitCalculator;
        private readonly IUsersRepository usersRepository;
        private readonly ITransactionsRepository transactionsRepository;
        private readonly IMaintainanceJobsRepository jobsRepository;

        public MaintainTransactionsUpdated(
            ILogger<MaintainTransactionsUpdated> logger,
            IStockMarketProfitCalculator profitCalculator,
            IUsersRepository usersRepository,
            ITransactionsRepository transactionsRepository,
            IMaintainanceJobsRepository jobsRepository)
        {
            _logger = logger;
            this.profitCalculator = profitCalculator;
            this.usersRepository = usersRepository;
            this.transactionsRepository = transactionsRepository;
            this.jobsRepository = jobsRepository;
        }

        public async Task UpdateTransactions()
        {
            _logger.LogWarning($"[Update transactions task] Started monitoring transactions {DateTimeOffset.UtcNow}!");

            var openedTransactions = transactionsRepository.GetOpenTransactions();

            foreach (var transaction in openedTransactions)
            {
                decimal profit = profitCalculator.CalculateTransactionProfit(transaction);

                if (profit <= -transaction.StopLossAmount ||
                    profit >= transaction.TakeProfitAmount)
                    await usersRepository.CloseUserTransaction(transaction, profit);
            }
            jobsRepository.MarkJobFinished(MaintainanceTasksSchedulerHelpers.TransactionMonitorJob);

            _logger.LogWarning($"[Update transactions task] Done monitoring transactions! {DateTimeOffset.UtcNow}");
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await UpdateTransactions();
        }
    }
}
