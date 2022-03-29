using System.Threading.Tasks;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksProccesing.Relational.Model;

namespace StocksProccesing.Relational.DataAccess.V1;

public class OrdersRepository : Repository<Order, int>, IOrdersRepository
{
    public OrdersRepository(StocksMarketContext context) : base(context)
    {
    }

    public async Task PlaceRefillBalanceOrder(ApplicationUser user, Order order)
    {
        user.Capital += order.Amount;

        await _dbContext.Orders.AddAsync(order);

        await _dbContext.SaveChangesAsync();
    }
}