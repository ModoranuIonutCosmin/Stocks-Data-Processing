using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Model;
using System;
using System.Linq;

namespace StocksProccesing.Relational.Repositories
{
    public class TransactionsRepository : ITransactionsRepository
    {

        public StocksMarketContext _dbContext { get; set; }

        public TransactionsRepository(StocksMarketContext dbContext)
        {
            _dbContext = dbContext;
        }

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
