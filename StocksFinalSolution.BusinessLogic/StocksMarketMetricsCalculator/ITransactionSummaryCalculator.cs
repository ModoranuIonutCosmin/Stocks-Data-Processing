using Stocks.General.Models;
using StocksProccesing.Relational.Model;
using System.Collections.Generic;

namespace StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator
{
    public interface ITransactionSummaryCalculator
    {
        List<CompanyTransactionsSummary> AggregateOpenTransactionsDataByCompaniesInfo(List<StocksTransaction> transactions);
        AllTransactionsDetailed AggregateOpenTransactionsDataForSingleCompany(List<StocksTransaction> transactions,
            string ticker);
    }
}