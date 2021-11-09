using Microsoft.EntityFrameworkCore;
using StocksProccesing.Relational.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StocksProccesing.Relational.DataAccess.V1.Repositories
{
    public class UsersRepository : Repository<ApplicationUser, string>, IUsersRepository
    {
        public UsersRepository(StocksMarketContext context) : base(context)
        {

        }

        public StocksTransaction GetTransactionInfo(int id)
        => _dbContext.Transactions
               .First(e => e.Id == id);

        public List<StocksTransaction> GetTransactionsListForUser(ApplicationUser user)
        {
            return _dbContext.Transactions
                    .Where(e => e.ApplicationUserId == user.Id && e.Open == true)
                    .AsNoTracking()
                    .ToList();
        }

        public List<StocksTransaction> GetTransactionsListForUserByTicker(ApplicationUser user, string ticker)
        {
            return _dbContext.Transactions
                    .Where(e => e.ApplicationUserId == user.Id && e.Open == true
                    && e.Ticker == ticker)
                    .ToList();
        }

        public async Task<bool> OpenUserTransaction(ApplicationUser user, StocksTransaction transaction)
        {
            try
            {
                _dbContext.Transactions.Add(transaction);
                user.Capital -= transaction.InvestedAmount;

                await _dbContext.SaveChangesAsync();
            }
            catch
            {

                return false;
            }

            return true;
        }
        public async Task CloseUserTransaction(StocksTransaction transaction,
            decimal profitOrLoss)
        {
            ApplicationUser user = await GetByIdAsync(transaction.ApplicationUserId);

            user.Capital += transaction.InvestedAmount + profitOrLoss;
            transaction.Open = false;

            await _dbContext.SaveChangesAsync();
        }
    }
}
