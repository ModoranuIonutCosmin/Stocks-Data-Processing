using System.Collections.Generic;
using System.Linq;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksProccesing.Relational.Model;

namespace StocksProccesing.Relational.DataAccess.V1;

public class TransactionsRepository : Repository<StocksTransaction, int>,
    ITransactionsRepository
{
    public TransactionsRepository(StocksMarketContext context) : base(context)
    {
    }

    public List<StocksTransaction> GetOpenTransactions()
    {
        return _dbContext.Transactions
            .Where(e => e.Open)
            .ToList();
    }


    public bool ExistsTransaction(string uniqueTransactionToken)
    {
        try
        {
            if (_dbContext.Transactions
                .Any(e => e.UniqueActionStamp == uniqueTransactionToken))
                return true;
            return false;
        }
        catch
        {
            return false;
        }
    }
}