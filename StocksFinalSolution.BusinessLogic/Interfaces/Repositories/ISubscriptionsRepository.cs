using System.Threading.Tasks;
using Stocks.General.Entities;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories.Base;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Repositories;

public interface ISubscriptionsRepository : IRepository<Subscription, string>
{
    public Task<ApplicationUser> AssignCustomerIdToUser(string userId, string customerId);
    public Task<Subscription> GetSubscriptionByCustomerId(string customerId);
}