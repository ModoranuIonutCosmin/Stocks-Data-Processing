﻿using Microsoft.Extensions.Logging;
using Quartz;
using Stocks.General.ConstantsConfig;
using Stocks.General.ExtensionMethods;
using Stocks_Data_Processing.Actions;
using StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.DataAccess.V1.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Stocks_Data_Processing.Utilities
{
    public class MaintainTaxesCollected : IMaintainTaxesCollected
    {
        private readonly StocksMarketContext _dbContext;
        private readonly IStockMarketOrderTaxesCalculator taxesCalculator;
        private readonly IMaintainanceJobsRepository jobsRepository;
        private readonly IUsersRepository usersRepository;
        private readonly ITransactionsRepository transactionsRepository;
        private readonly ILogger<MaintainTaxesCollected> _logger;

        public MaintainTaxesCollected(
            IStockMarketOrderTaxesCalculator taxesCalculator,
            IMaintainanceJobsRepository jobsRepository,
            IUsersRepository usersRepository,
            ITransactionsRepository transactionsRepository,
            ILogger<MaintainTaxesCollected> logger)
        {
            //_dbContext = contextFactory.Create();
            this.taxesCalculator = taxesCalculator;
            this.jobsRepository = jobsRepository;
            this.usersRepository = usersRepository;
            this.transactionsRepository = transactionsRepository;
            _logger = logger;
        }

        public async Task CollectTaxes()
        {
            _logger.LogWarning($"[Tax collection task] Started charging taxes {DateTimeOffset.UtcNow}!");

            var taskInfo = jobsRepository
                .GetMaintenanceActionByName(MaintainanceTasksSchedulerHelpers.TaxesCollectJob);

            var lastGlobalUpdate = taskInfo.LastFinishedDate;

            var allTaxableTransactions = transactionsRepository.GetOpenTransactions()
                .Where(e => !e.IsBuy || e.Leverage > 1)
                .ToList();

            foreach (var transaction in allTaxableTransactions)
            {
                var taxesOwed = taxesCalculator.CalculateTaxes(transaction, lastGlobalUpdate);
                    
                usersRepository.SubtractCapital(transaction.ApplicationUserId, taxesOwed, false);
            }

            jobsRepository.MarkJobFinished(MaintainanceTasksSchedulerHelpers.TaxesCollectJob);

            await usersRepository.SaveChangesAsync();

            _logger.LogWarning($"[Tax collection task] Done collecting taxes! {DateTimeOffset.UtcNow}");
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await CollectTaxes();
        }
    }
}
