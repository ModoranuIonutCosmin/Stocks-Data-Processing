
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Stocks.General.Models.Payments;
using StocksProccesing.Relational.Model;
using StocksProcessing.API.Attributes;
using Stripe;
using Stripe.Checkout;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StocksProcessing.API.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    public partial class PaymentCheckoutController : BaseController
    {
        private readonly IOptions<StripeSettings> _stripeConfiguration;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentCheckoutController(IConfiguration configuration,
            IOptions<StripeSettings> stripeConfiguration,
            UserManager<ApplicationUser> userManager)
        {
            _stripeConfiguration = stripeConfiguration;
            _userManager = userManager;
        }

        [HttpPost("create-checkout-session")]
        [AuthorizeToken]
        public async Task<IActionResult> Create([FromBody] CreateCheckoutSessionRequest request)
        {
            ApplicationUser userRequesting = await _userManager.GetUserAsync(HttpContext.User);


            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    Price = request.PriceId,
                    Quantity = 1,
                  },
                },

                Mode = "subscription",
                SuccessUrl = request.FrontEndSuccesUrl,
                CancelUrl = request.FrontEndFailureUrl,
                Metadata = new Dictionary<string, string>
                {
                   {"UserId", userRequesting.Id},
                }
            };
            var service = new SessionService();

            Session session = service.Create(options);

            return Ok(new CreateCheckoutSessionResponse
            {
                SessionId = session.Id,
                PublicKey = _stripeConfiguration.Value.PublicKey
            });
        }


    }
}