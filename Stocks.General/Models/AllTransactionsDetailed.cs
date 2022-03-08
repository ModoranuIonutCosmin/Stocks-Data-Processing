using System.Collections.Generic;

namespace Stocks.General.Models
{
    public class AllTransactionsDetailed
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string UrlLogo { get; set; }
        public string Ticker { get; set; }
        public List<TransactionFullInfo> Transactions { get; set; } = new List<TransactionFullInfo>();
    }
}
