using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Model;
using System.Threading.Tasks;

namespace StocksProccesing.Relational.Repositories
{
    public class OrdersRepository : IOrdersRepository
    {
        public StocksMarketContext _dbContext { get; set; }

        public OrdersRepository(StocksMarketContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task PlaceRefillBalanceOrder(ApplicationUser user, Order order)
        {
            user.Capital += order.Amount;

            await _dbContext.Orders.AddAsync(order);

            await _dbContext.SaveChangesAsync();
        }
    }
}
