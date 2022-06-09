using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Stocks_Data_Processing.Actions;
using Stocks_Data_Processing.Interfaces.Jobs;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;
using StocksProccesing.Relational.Model;

namespace Stocks_Data_Processing.Jobs;

public class MaintainTransactionsUpdated : IMaintainTransactionsUpdated
{
    private readonly ILogger<MaintainTransactionsUpdated> _logger;
    private readonly IMaintainanceJobsRepository jobsRepository;
    private readonly IStockMarketProfitCalculator profitCalculator;
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly IUsersRepository _usersRepository;

    public MaintainTransactionsUpdated(
        ILogger<MaintainTransactionsUpdated> logger,
        IStockMarketProfitCalculator profitCalculator,
        IUsersRepository usersRepository,
        ITransactionsRepository transactionsRepository,
        IMaintainanceJobsRepository jobsRepository)
    {
        _logger = logger;
        this.profitCalculator = profitCalculator;
        this._usersRepository = usersRepository;
        this._transactionsRepository = transactionsRepository;
        this.jobsRepository = jobsRepository;
    }

    public async Task UpdateTransactions()
    {
        _logger.LogWarning($"[Update transactions task] Started monitoring transactions {DateTimeOffset.UtcNow}!");

        var openedTransactions = _transactionsRepository.GetOpenTransactions();

        foreach (var transaction in openedTransactions)
        {

            if (transaction.Open)
            {
                await HandleTransactionsWithExtendedTradingParams(transaction);
                await HandleTransactionsWithScheduledAutoClose(transaction);
            } else
            {
                await HandleTransactionsWithScheduledAutoOpen(transaction);
            }
            //
        }

        jobsRepository.MarkJobFinished(MaintainanceTasksSchedulerHelpers.TransactionMonitorJob);

        _logger.LogWarning($"[Update transactions task] Done monitoring transactions! {DateTimeOffset.UtcNow}");
    }


    private async Task HandleTransactionsWithExtendedTradingParams(StocksTransaction transaction)
    {
        var profit = profitCalculator.CalculateTransactionProfit(transaction);
        var tradeValue = transaction.InvestedAmount + profit;

        if (tradeValue <= transaction.StopLossAmount ||
            tradeValue >= transaction.TakeProfitAmount)
            await _usersRepository.CloseUserTransaction(transaction, profit);
    }

    private async Task HandleTransactionsWithScheduledAutoClose(StocksTransaction transaction)
    {
        if (transaction.ScheduledAutoClose == default(DateTimeOffset))
        {
            return;
        }

        if (transaction.ScheduledAutoClose < DateTimeOffset.UtcNow)
        {
            var profit = profitCalculator.CalculateTransactionProfit(transaction);

            await _usersRepository.CloseUserTransaction(transaction, profit);
        }
    }

    private async Task HandleTransactionsWithScheduledAutoOpen(StocksTransaction transaction)
    {
        if (transaction.ScheduledAutoOpen == default(DateTimeOffset))
        {
            return;
        }

        if (transaction.ScheduledAutoOpen < DateTimeOffset.UtcNow)
        {
            ApplicationUser applicationUser = await _usersRepository.GetByIdAsync(transaction.ApplicationUserId);

            if (applicationUser.Capital < transaction.InvestedAmount)
            {
                transaction.ScheduledAutoOpen = default;

                await _transactionsRepository.UpdateAsync(transaction);

                return;
            } 
            else
            {
                applicationUser.Capital -= transaction.InvestedAmount;
                transaction.Open = true;

                await _usersRepository.UpdateAsync(applicationUser);
                await _transactionsRepository.UpdateAsync(transaction);
            }
        }
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await UpdateTransactions();
    }
}