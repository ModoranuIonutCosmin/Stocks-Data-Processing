using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Stocks_Data_Processing.Actions;
using Stocks_Data_Processing.Interfaces.Jobs;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;

namespace Stocks_Data_Processing.Jobs;

public class MaintainTaxesCollected : IMaintainTaxesCollected
{
    private readonly ILogger<MaintainTaxesCollected> _logger;
    private readonly IMaintainanceJobsRepository jobsRepository;
    private readonly IStockMarketOrderTaxesCalculator taxesCalculator;
    private readonly ITransactionsRepository transactionsRepository;
    private readonly IUsersRepository usersRepository;

    public MaintainTaxesCollected(
        IStockMarketOrderTaxesCalculator taxesCalculator,
        IMaintainanceJobsRepository jobsRepository,
        IUsersRepository usersRepository,
        ITransactionsRepository transactionsRepository,
        ILogger<MaintainTaxesCollected> logger)
    {
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