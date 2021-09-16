using Quartz;
using System.Linq;
using System.Threading.Tasks;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Helpers;
using Stocks.General.ConstantsConfig;
using Stocks.General.ExtensionMethods;
using Microsoft.Extensions.Logging;
using System;

namespace Stocks_Data_Processing.Utilities
{
    public class MaintainTransactionsUpdated : IJob, IMaintainTransactionsUpdated
    {
        private readonly StocksMarketContext _dbContext;
        private readonly ILogger<MaintainTransactionsUpdated> _logger;

        public MaintainTransactionsUpdated(StockContextFactory contextFactory,
            ILogger<MaintainTransactionsUpdated> logger)
        {
            _dbContext = contextFactory.Create();
            _logger = logger;
        }

        public async Task UpdateTransactions()
        {
            _logger.LogWarning($"[Update transactions task] Started monitoring transactions {DateTimeOffset.UtcNow}!");

            var openedTransactions = _dbContext.Transactions
                .Where(e => e.Open)
                .ToList();

            foreach (var transaction in openedTransactions)
            {

                var ticker = transaction.Ticker;
                var profit = default(double);

                var currentSellPrice = _dbContext.GatherCurrentPriceByCompany(ticker);

                var spreadTotalFees = TaxesConfig.AverageStockMarketSpread;
                var currentBuyPrice = currentSellPrice + currentSellPrice * spreadTotalFees;

                if (transaction.IsBuy)
                {
                    var unitsPurchased = transaction.InvestedAmount / transaction.UnitBuyPriceThen;

                    profit = ((currentSellPrice - transaction.UnitBuyPriceThen) * unitsPurchased).TruncateToDecimalPlaces(3);
                }
                else
                {
                    var unitsPurchased = transaction.InvestedAmount / transaction.UnitSellPriceThen;

                    profit = ((currentBuyPrice - transaction.UnitSellPriceThen) * unitsPurchased).TruncateToDecimalPlaces(3);
                }

                if (profit <= -transaction.StopLossAmount || profit >= transaction.TakeProfitAmount)
                {
                    transaction.Open = false;

                    var userRequesting = _dbContext.Users
                        .Where(e => e.Id == transaction.ApplicationUserId)
                        .Last();

                    userRequesting.Capital += profit;
                }

                await _dbContext.SaveChangesAsync();
            }

            _logger.LogWarning($"[Update transactions task] Done monitoring transactions! {DateTimeOffset.UtcNow}");
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await UpdateTransactions();
        }
    }
}
