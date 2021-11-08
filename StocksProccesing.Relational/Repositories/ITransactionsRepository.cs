using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Model;
using System.Collections.Generic;

namespace StocksProccesing.Relational.Repositories
{
    public interface ITransactionsRepository : IEFRepository<StocksMarketContext>
    {
        bool ExistsTransaction(string uniqueTransactionToken);
        List<StocksTransaction> GetOpenTransactions();
    }
}