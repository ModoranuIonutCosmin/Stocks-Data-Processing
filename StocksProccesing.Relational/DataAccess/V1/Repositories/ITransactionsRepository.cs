using StocksProccesing.Relational.Interfaces;
using StocksProccesing.Relational.Model;
using System.Collections.Generic;

namespace StocksProccesing.Relational.Repositories
{
    public interface ITransactionsRepository : IRepository<StocksTransaction, int>
    {
        bool ExistsTransaction(string uniqueTransactionToken);
        List<StocksTransaction> GetOpenTransactions();
    }
}