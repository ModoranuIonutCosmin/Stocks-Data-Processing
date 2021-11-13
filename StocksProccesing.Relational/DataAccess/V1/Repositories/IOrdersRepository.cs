using StocksProccesing.Relational.Interfaces;
using StocksProccesing.Relational.Model;
using System.Threading.Tasks;

namespace StocksProccesing.Relational.DataAccess.V1.Repositories
{
    public interface IOrdersRepository : IRepository<Order, int>
    {
        Task PlaceRefillBalanceOrder(ApplicationUser user, Order order);
    }
}