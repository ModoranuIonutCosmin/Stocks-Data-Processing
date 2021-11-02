using StocksProccesing.Relational.DataAccess;

namespace StocksProccesing.Relational.Repositories
{
    public interface ITransactionsRepository : IEFRepository<StocksMarketContext>
    {
        bool ExistsTransaction(string uniqueTransactionToken);
    }
}