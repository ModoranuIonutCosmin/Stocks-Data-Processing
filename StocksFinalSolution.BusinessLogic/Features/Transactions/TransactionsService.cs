using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Stocks.General.Exceptions;
using Stocks.General.Models.Transactions;
using StocksFinalSolution.BusinessLogic.Features.Authentication.ExtensionMethods;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;
using StocksProccesing.Relational.Model;
using StocksProcessing.General.Exceptions;

namespace StocksFinalSolution.BusinessLogic.Features.Transactions;

public class TransactionsService : ITransactionsService
{
    private readonly ITransactionSummaryCalculator _transactionSummaryCalculator;
    private readonly IUsersRepository _usersRepository;
    private readonly IStockMarketProfitCalculator _stockMarketProfitCalculator;

    public TransactionsService(ITransactionSummaryCalculator transactionSummaryCalculator,
        IUsersRepository usersRepository,
        IStockMarketProfitCalculator stockMarketProfitCalculator)
    {
        _transactionSummaryCalculator = transactionSummaryCalculator;
        _usersRepository = usersRepository;
        _stockMarketProfitCalculator = stockMarketProfitCalculator;
    }


    public async Task<AllTransactionsGroupedSummary> GatherTransactionsSummary(ApplicationUser userRequesting)
    {
        List<TransactionSummary> transactionsSummary;
        transactionsSummary =
            await _transactionSummaryCalculator
                .AggregateOpenTransactionsDataByCompaniesInfoAsync(
                    _usersRepository.GetTransactionsListForUser(userRequesting)
                );

        return new AllTransactionsGroupedSummary
        {
            Transactions = transactionsSummary
        };
    }


    public async Task<AllTransactionsDetailed> GatherTransactionsParticularTicker(ApplicationUser userRequesting,
        string ticker)
    {
        List<StocksTransaction> openTransactionsList = _usersRepository
            .GetTransactionsListForUserByTicker(userRequesting, ticker);

        var result = _transactionSummaryCalculator
            .AggregateOpenTransactionsDataForSingleCompany(openTransactionsList, ticker);


        return result;
    }


    public async Task<CloseTransactionRequest> CancelTransaction(ApplicationUser userRequesting,
        CloseTransactionRequest request)
    {
        var transaction = _usersRepository.GetTransactionInfo(request.Id);

        if (transaction is null)
        {
            throw new InvalidTransactionException(nameof(transaction) + "transaction were not found!");
        }

        if (!userRequesting.UserOwnsTheTransaction(transaction))
        {
            throw new InvalidTransactionOwner("User does not own this transaction");
        }

        decimal profitOrLoss = _stockMarketProfitCalculator.CalculateTransactionProfit(transaction);

        await _usersRepository.CloseUserTransaction(transaction, profitOrLoss);

        return request;
    }
}