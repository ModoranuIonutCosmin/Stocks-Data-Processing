using System.Threading.Tasks;
using Stocks.General.Models.Transactions;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Services;

public interface ITransactionsService
{
    Task<AllTransactionsGroupedSummary> GatherTransactionsSummary(ApplicationUser userRequesting);

    Task<AllTransactionsDetailed> GatherTransactionsParticularTicker(ApplicationUser userRequesting,
        string ticker);

    Task<CloseTransactionRequest> CancelTransaction(ApplicationUser userRequesting,
        CloseTransactionRequest request);
}