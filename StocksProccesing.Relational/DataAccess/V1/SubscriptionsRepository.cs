using Microsoft.EntityFrameworkCore;
using Stocks.General.Entities;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksProccesing.Relational.Model;
using System.Threading.Tasks;

namespace StocksProccesing.Relational.DataAccess.V1;
public class SubscriptionsRepository : Repository<Subscription, string>, ISubscriptionsRepository
{
    public SubscriptionsRepository(StocksMarketContext context) : base(context)
    {

    }

    public async Task<ApplicationUser> AssignCustomerIdToUser(string userId, string customerId)
    {
        ApplicationUser user = await _dbContext.FindAsync<ApplicationUser>(userId);

        user.CustomerId = customerId;

        await SaveChangesAsync();

        return user;
    }

    public Task<Subscription> GetSubscriptionByCustomerId(string customerId)
    {
        return _dbContext.Subscriptions.SingleOrDefaultAsync(s => s.CustomerId == customerId);
    }
}