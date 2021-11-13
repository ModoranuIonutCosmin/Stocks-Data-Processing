using System.Collections.Generic;

namespace Stocks.General.Models
{
    public class AllTransactionsGroupedSummary
    {
        public List<TransactionSummary> Transactions { get; set; } = new List<TransactionSummary>();
    }
}
