using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.DataAccess.V1;
using StocksProccesing.Relational.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StocksProccesing.Relational.Repositories
{
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
}
