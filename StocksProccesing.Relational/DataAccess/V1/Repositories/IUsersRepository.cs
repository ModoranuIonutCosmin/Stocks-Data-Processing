using StocksProccesing.Relational.Interfaces;
using StocksProccesing.Relational.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StocksProccesing.Relational.DataAccess.V1.Repositories
{
    public interface IUsersRepository : IRepository<ApplicationUser, string>
    {
        Task CloseUserTransaction(StocksTransaction transaction, decimal profitOrLoss);
        StocksTransaction GetTransactionInfo(int id);
        List<StocksTransaction> GetTransactionsListForUser(ApplicationUser user);
        List<StocksTransaction> GetTransactionsListForUserByTicker(ApplicationUser user, string ticker);
        Task<bool> OpenUserTransaction(ApplicationUser user, StocksTransaction transaction);
    }
}