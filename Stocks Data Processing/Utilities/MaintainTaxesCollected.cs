using Microsoft.Extensions.Logging;
using Quartz;
using Stocks.General.ConstantsConfig;
using Stocks.General.ExtensionMethods;
using StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Stocks_Data_Processing.Utilities
{
    public class MaintainTaxesCollected : IMaintainTaxesCollected
    {
        private readonly StocksMarketContext _dbContext;
        private readonly IStockMarketOrderTaxesCalculator taxesCalculator;
        private readonly IUsersRepository usersRepository;
        private readonly ITransactionsRepository transactionsRepository;
        private readonly ILogger<MaintainTaxesCollected> _logger;

        public MaintainTaxesCollected(StockContextFactory contextFactory,
            IStockMarketOrderTaxesCalculator taxesCalculator,
            IUsersRepository usersRepository,
            ITransactionsRepository transactionsRepository,
            ILogger<MaintainTaxesCollected> logger)
        {
            _dbContext = contextFactory.Create();
            this.taxesCalculator = taxesCalculator;
            this.usersRepository = usersRepository;
            this.transactionsRepository = transactionsRepository;
            _logger = logger;
        }

        public async Task CollectTaxes()
        {
            _logger.LogWarning($"[Tax collection task] Started charging taxes {DateTimeOffset.UtcNow}!");

            var maintananceActionsData = _dbContext.Actions
                .Where(e => e.Type == MaintananceJobsName.TaxesCollectingJob)
                .ToList();

            var currentDate = DateTimeOffset.UtcNow;
            var lastGlobalUpdate = DateTimeOffset.FromUnixTimeSeconds(0);

            if (maintananceActionsData.Count > 0)
            {
                lastGlobalUpdate = maintananceActionsData.Last().LastFinishedDate;
            }

            var allTaxableTransactions = transactionsRepository.GetOpenTransactions()
                .Where(e => !e.IsBuy || e.Leverage > 1)
                .ToList();

            foreach (var transaction in allTaxableTransactions)
            {
                var dateTransactionUpdated = lastGlobalUpdate < transaction.Date ? transaction.Date
                                                                                 : lastGlobalUpdate;

                var weekDays = DateTimeOffsetHelpers.GetBusinessDays(dateTransactionUpdated, currentDate);
                var weekendDays = (decimal)currentDate.Subtract(dateTransactionUpdated).TotalDays - weekDays;

                var borrowedMoney = transaction.Leverage * transaction.InvestedAmount - transaction.InvestedAmount;

                var weekdayTax = taxesCalculator.CalculateWeekDayTax(transaction.Leverage, borrowedMoney, transaction.IsBuy);
                var weekendTax = taxesCalculator.CalculateWeekEndTax(transaction.Leverage, borrowedMoney, transaction.IsBuy);

                var requestingUser = await usersRepository.GetByIdAsync(transaction.ApplicationUserId);

                requestingUser.Capital -= weekDays * weekdayTax + weekendDays * weekendTax;

                await _dbContext.SaveChangesAsync();
            }

            //Flaw aici 

            _logger.LogWarning($"[Tax collection task] Done collecting taxes! {DateTimeOffset.UtcNow}");
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await CollectTaxes();
        }
    }
}
