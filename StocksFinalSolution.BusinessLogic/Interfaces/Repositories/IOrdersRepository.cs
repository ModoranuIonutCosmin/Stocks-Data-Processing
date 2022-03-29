using System.Threading.Tasks;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories.Base;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Repositories;

public interface IOrdersRepository : IRepository<Order, int>
{
    Task PlaceRefillBalanceOrder(ApplicationUser user, Order order);
}