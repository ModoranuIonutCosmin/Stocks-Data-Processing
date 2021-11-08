using Stocks.General.Models;
using StocksProccesing.Relational.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator
{
    public interface ITransactionSummaryCalculator
    {
        Task<List<CompanyTransactionsSummary>> AggregateOpenTransactionsDataByCompaniesInfoAsync(List<StocksTransaction> transactions);
        AllTransactionsDetailed AggregateOpenTransactionsDataForSingleCompany(List<StocksTransaction> transactions,
            string ticker);
    }
}