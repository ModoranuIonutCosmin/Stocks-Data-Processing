using Microsoft.Extensions.Logging;
using Quartz;
using Stocks.General.ConstantsConfig;
using Stocks.General.ExtensionMethods;
using StocksProccesing.Relational.DataAccess;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Stocks_Data_Processing.Utilities
{
    public class MaintainTaxesCollected : IMaintainTaxesCollected, IJob
    {
        private readonly StocksMarketContext _dbContext;
        private readonly ILogger<MaintainTaxesCollected> _logger;

        public MaintainTaxesCollected(StockContextFactory contextFactory,
            ILogger<MaintainTaxesCollected> logger)
        {
            _dbContext = contextFactory.Create();
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

            var allTaxableTransactions = _dbContext
                .Transactions
                .Where(e => e.Open && (!e.IsBuy || e.Leverage > 1))
                .ToList();

            foreach (var transaction in allTaxableTransactions)
            {
                var dateTransactionUpdated = lastGlobalUpdate < transaction.Date ? transaction.Date
                                                                                 : lastGlobalUpdate;

                var workDays = DateTimeOffsetHelpers.GetBusinessDays(dateTransactionUpdated, currentDate);
                var weekEndDays = (decimal)currentDate.Subtract(dateTransactionUpdated).TotalDays - workDays;

                //dangerous cast

                var interestAmount = transaction.IsBuy ? TaxesConfig.BuyInterestRate : TaxesConfig.SellInterestRate;

                var weekdayTax = ((interestAmount + BankExchangeConsts.LiborOneMonthRatio) / 365 * transaction.InvestedAmount)
                .TruncateToDecimalPlaces(3);
                var weekEndTax = weekdayTax * TaxesConfig.WeekendOvernightMultiplier;

                var requestingUser = _dbContext.Users
                    .Where(e => e.Id == transaction.ApplicationUserId)
                    .First();

                requestingUser.Capital -= workDays * weekdayTax + weekEndDays * weekEndTax;

                await _dbContext.SaveChangesAsync();
            }

            _logger.LogWarning($"[Tax collection task] Done collecting taxes! {DateTimeOffset.UtcNow}");
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await CollectTaxes();
        }
    }
}
