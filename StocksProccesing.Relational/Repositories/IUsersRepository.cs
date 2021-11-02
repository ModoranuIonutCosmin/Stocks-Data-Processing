using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Model;
using StocksProcessing.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StocksProccesing.Relational.Repositories
{
    public interface IUsersRepository : IEFRepository<StocksMarketContext>
    {
        Task CloseUserTransaction(ApplicationUser user, StocksTransaction transaction, decimal profitOrLoss);
        StocksTransaction GetTransactionInfo(int id);
        List<StocksTransaction> GetTransactionsListForUser(ApplicationUser user);
        List<StocksTransaction> GetTransactionsListForUserByTicker(ApplicationUser user, string ticker);
        Task<bool> OpenUserTransaction(ApplicationUser user, StocksTransaction transaction);
    }
}