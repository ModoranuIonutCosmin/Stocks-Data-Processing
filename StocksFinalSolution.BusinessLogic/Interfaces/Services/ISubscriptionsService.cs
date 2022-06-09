using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stripe;

namespace StocksFinalSolution.BusinessLogic.Interfaces;
public interface ISubscriptionsService
{

    Task HandleSubscriptionCreated (Stripe.Checkout.Session checkoutSession);

    Task HandleSubscriptionPaymentStatus(Invoice invoice);
    Task HandleNewCustomer(Stripe.Checkout.Session session);
}