using System.Collections.Generic;
using System.Threading.Tasks;
using Stocks.General.Models;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Services
{
    public interface ITransactionSummaryCalculator
    {
        Task<List<TransactionSummary>> AggregateOpenTransactionsDataByCompaniesInfoAsync(List<StocksTransaction> transactions);
        AllTransactionsDetailed AggregateOpenTransactionsDataForSingleCompany(List<StocksTransaction> transactions,
            string ticker);
    }
}