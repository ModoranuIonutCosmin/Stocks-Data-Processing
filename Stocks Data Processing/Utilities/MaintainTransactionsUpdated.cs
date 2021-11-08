using Microsoft.Extensions.Logging;
using Quartz;
using StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Repositories;
using System;
using System.Threading.Tasks;

namespace Stocks_Data_Processing.Utilities
{
    public class MaintainTransactionsUpdated : IMaintainTransactionsUpdated
    {
        private readonly StocksMarketContext _dbContext;
        private readonly ILogger<MaintainTransactionsUpdated> _logger;
        private readonly IStockMarketProfitCalculator profitCalculator;
        private readonly IUsersRepository usersRepository;
        private readonly ITransactionsRepository transactionsRepository;

        public MaintainTransactionsUpdated(StockContextFactory contextFactory,
            ILogger<MaintainTransactionsUpdated> logger,
            IStockMarketProfitCalculator profitCalculator,
            IUsersRepository usersRepository,
            ITransactionsRepository transactionsRepository)
        {
            _dbContext = contextFactory.Create();
            _logger = logger;
            this.profitCalculator = profitCalculator;
            this.usersRepository = usersRepository;
            this.transactionsRepository = transactionsRepository;
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

            _logger.LogWarning($"[Update transactions task] Done monitoring transactions! {DateTimeOffset.UtcNow}");
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await UpdateTransactions();
        }
    }
}
