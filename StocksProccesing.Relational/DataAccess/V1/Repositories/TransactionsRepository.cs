using StocksProccesing.Relational.Model;
using System.Collections.Generic;
using System.Linq;

namespace StocksProccesing.Relational.DataAccess.V1.Repositories
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
                else return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
