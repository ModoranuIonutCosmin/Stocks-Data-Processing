
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StocksFinalSolution.BusinessLogic.Interfaces;
using Stripe;
using System;
using System.IO;
using System.Threading.Tasks;

namespace StocksProcessing.API.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    public class StripeWebhookController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly ISubscriptionsService _subscriptionsService;

        public StripeWebhookController(IConfiguration configuration,
            ISubscriptionsService subscriptionsService)
        {
            _configuration = configuration;
            _subscriptionsService = subscriptionsService;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            string endpointSecret = _configuration["Stripe:EventWebhook"];

            try
            {
                var stripeEvent = EventUtility.ParseEvent(json);
                var signatureHeader = Request.Headers["Stripe-Signature"];

                stripeEvent = EventUtility.ConstructEvent(json,
                        signatureHeader, endpointSecret);

                // Handle the event
                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {   
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;


                    await _subscriptionsService.HandleSubscriptionCreated(session);
                }
                else if (stripeEvent.Type == Events.InvoicePaid)
                {
                    var invoice = stripeEvent.Data.Object as Invoice;

                    await _subscriptionsService.HandleSubscriptionPaymentStatus(invoice);
                }
                else if (stripeEvent.Type == Events.InvoicePaymentFailed)
                {
                    var invoice = stripeEvent.Data.Object as Invoice;

                    await _subscriptionsService.HandleSubscriptionPaymentStatus(invoice);
                }
                // ... handle other event types
                else
                {
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }
                return Ok();
            }
            catch (StripeException e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return BadRequest();
            }
        }
    }
}