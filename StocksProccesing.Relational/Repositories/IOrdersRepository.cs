using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Model;
using System.Threading.Tasks;

namespace StocksProccesing.Relational.Repositories
{
    public interface IOrdersRepository : IEFRepository<StocksMarketContext>
    {
        Task PlaceRefillBalanceOrder(ApplicationUser user, Order order);
    }
}