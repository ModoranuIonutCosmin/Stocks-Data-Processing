using StocksFinalSolution.BusinessLogic.Interfaces;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksProccesing.Relational.Model;
using Stripe;
using Stripe.Checkout;
using System.Threading.Tasks;
using DbDataModels = global::Stocks.General.DataModels;
using DbEntities = global::Stocks.General.Entities;

namespace StocksFinalSolution.BusinessLogic.Features.Subscriptions
{
    public class SubscriptionsService : ISubscriptionsService
    {
        private readonly ISubscriptionsRepository _subscriptionsRepository;
        private readonly IUsersRepository _usersRepository;

        public SubscriptionsService(ISubscriptionsRepository subscriptionsRepository,
            IUsersRepository usersRepository)
        {
            _subscriptionsRepository = subscriptionsRepository;
            _usersRepository = usersRepository;
        }

        public async Task HandleNewCustomer(Stripe.Checkout.Session session)
        {
            ApplicationUser customerIdentity = await _usersRepository.GetByIdAsync(session.Metadata["UserId"]);

            await _subscriptionsRepository.AssignCustomerIdToUser(customerIdentity.Id, session.CustomerId);
        }

        public async Task HandleSubscriptionCreated(Stripe.Checkout.Session checkoutSession)
        {
            await HandleNewCustomer(checkoutSession);

            await UpdateCurrentSubscription(checkoutSession.SubscriptionId);
        }

        public async Task HandleSubscriptionPaymentStatus(Invoice invoice)
        {
            await UpdateCurrentSubscription(invoice.SubscriptionId);
        }


        public async Task UpdateCurrentSubscription(string subscriptionId)
        {
            var service = new SubscriptionService();

            Subscription subscriptionDetails = service.Get(subscriptionId);

            string customerId = subscriptionDetails.CustomerId;
            DbEntities.Subscription existingSubscription
                = await _subscriptionsRepository.GetSubscriptionByCustomerId(customerId);

            if (existingSubscription == null)
            {
                await _subscriptionsRepository.AddAsync(new DbEntities.Subscription
                {
                    Id = subscriptionDetails.Id,
                    CustomerId = customerId,
                    PeriodStart = subscriptionDetails.CurrentPeriodStart,
                    PeriodEnd = subscriptionDetails.CurrentPeriodEnd,
                    Status = subscriptionDetails.Status,
                    Type = DbDataModels.SubscriptionType.PRO
                });

                return;
            }

            existingSubscription.Status = subscriptionDetails.Status;
            existingSubscription.PeriodStart = subscriptionDetails.CurrentPeriodStart;
            existingSubscription.PeriodEnd = subscriptionDetails.CurrentPeriodEnd;

            await _subscriptionsRepository.UpdateAsync(existingSubscription);
        }
    }
}
