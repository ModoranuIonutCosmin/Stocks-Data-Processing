using StocksProccesing.Relational.Interfaces;
using StocksProccesing.Relational.Model;
using System.Threading.Tasks;

namespace StocksProccesing.Relational.Repositories
{
    public interface IOrdersRepository : IRepository<Order, int>
    {
        Task PlaceRefillBalanceOrder(ApplicationUser user, Order order);
    }
}