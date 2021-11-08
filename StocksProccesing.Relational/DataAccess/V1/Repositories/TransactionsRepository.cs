using StocksProccesing.Relational.DataAccess;
using System.Linq;
using System.Collections.Generic;
using StocksProccesing.Relational.Model;
using StocksProccesing.Relational.DataAccess.V1;

namespace StocksProccesing.Relational.Repositories
{
    public class TransactionsRepository : Repository<StocksTransaction, int>,
        ITransactionsRepository
    {
        public TransactionsRepository(StocksMarketContext context) : base(context)
        {
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
