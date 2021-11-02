using System.Collections.Generic;

namespace Stocks.General.Models
{
    public class AllTransactionsGroupedSummary
    {
        public List<CompanyTransactionsSummary> Transactions { get; set; } = new List<CompanyTransactionsSummary>();
    }
}
