using System.Collections.Generic;

namespace Stocks.General.Models.Transactions;

public class AllTransactionsDetailed
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string UrlLogo { get; set; }
    public string Ticker { get; set; }
    public List<TransactionFullInfo> OpenTransactions { get; set; } = new();
    public List<TransactionFullInfo> ScheduledTransactions { get; set; } = new();
    public List<TransactionFullInfo> ClosedTransactions { get; set; } = new();
}