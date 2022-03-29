using System.Collections.Generic;

namespace Stocks.General.Models.Transactions;

public class AllTransactionsGroupedSummary
{
    public List<TransactionSummary> Transactions { get; set; } = new();
}