using System.Collections.Generic;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories.Base;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Repositories
{
    public interface ITransactionsRepository : IRepository<StocksTransaction, int>
    {
        bool ExistsTransaction(string uniqueTransactionToken);
        List<StocksTransaction> GetOpenTransactions();
    }
}