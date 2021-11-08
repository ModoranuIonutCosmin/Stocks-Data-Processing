using StocksProccesing.Relational.DataAccess;
using System.Linq;
using System.Collections.Generic;
using StocksProccesing.Relational.Model;

namespace StocksProccesing.Relational.Repositories
{
    public class TransactionsRepository : ITransactionsRepository
    {

        public StocksMarketContext _dbContext { get; set; }

        public TransactionsRepository(StocksMarketContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<StocksTransaction> GetOpenTransactions()
        => _dbContext.Transactions
                .Where(e => e.Open)
                .ToList();
        

        public bool ExistsTransaction(string uniqueTransactionToken)
        {
            try
            {
                if (_dbContext.Transactions
                    .Any(e => e.UniqueActionStamp == uniqueTransactionToken))
                    return true;
            }
            catch
            {

            }

            return false;
        }
    }
}
