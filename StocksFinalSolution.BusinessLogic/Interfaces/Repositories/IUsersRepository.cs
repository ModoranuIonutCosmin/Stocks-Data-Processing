using System.Collections.Generic;
using System.Threading.Tasks;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories.Base;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Repositories
{
    public interface IUsersRepository : IRepository<ApplicationUser, string>
    {
        Task CloseUserTransaction(StocksTransaction transaction, decimal profitOrLoss);
        StocksTransaction GetTransactionInfo(int id);
        List<StocksTransaction> GetTransactionsListForUser(ApplicationUser user);
        List<StocksTransaction> GetTransactionsListForUserByTicker(ApplicationUser user, string ticker);
        Task<bool> OpenUserTransaction(ApplicationUser user, StocksTransaction transaction);
        void SubtractCapital(string userId, decimal amount, bool transactional);
    }
}