using Microsoft.EntityFrameworkCore;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StocksProccesing.Relational.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        public StocksMarketContext _dbContext { get; set; }

        public UsersRepository(StocksMarketContext dbContext)
        {
            _dbContext = dbContext;
        }

        public StocksTransaction GetTransactionInfo(int id)
        => _dbContext.Transactions
               .First(e => e.Id == id);

        public ApplicationUser FindUserById(string id)
            => _dbContext.Users.First(u => u.Id == id);

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
            ApplicationUser user = FindUserById(transaction.ApplicationUserId);

            user.Capital += transaction.InvestedAmount + profitOrLoss;
            transaction.Open = false;

            await _dbContext.SaveChangesAsync();
        }
    }
}
