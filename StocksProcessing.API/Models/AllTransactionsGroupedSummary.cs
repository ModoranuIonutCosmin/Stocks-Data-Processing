using System.Collections.Generic;

namespace StocksProcessing.API.Models
{
    public class AllTransactionsGroupedSummary
    {
        public List<CompanyTransactionsSummary> Transactions { get; set; } = new List<CompanyTransactionsSummary>();
    }
}
