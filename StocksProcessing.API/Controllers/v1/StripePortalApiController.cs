
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StocksProccesing.Relational.Model;
using StocksProcessing.API.Attributes;
using Stripe.Checkout;
using System.Threading.Tasks;

namespace StocksProcessing.API.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    public class StripePortalApiController : BaseController
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<ApplicationUser> userManager;

        public StripePortalApiController(IConfiguration configuration,
            UserManager<ApplicationUser> userManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
        }

        [HttpPost("create-portal-session")]
        [AuthorizeToken]
        public async Task<IActionResult> Create()
        {
            var userRequesting = await userManager.GetUserAsync(HttpContext.User);

            var options = new Stripe.BillingPortal.SessionCreateOptions
            {
                Customer = userRequesting.CustomerId,
                ReturnUrl = configuration["FrontEnd:URL"],
            };

            var service = new Stripe.BillingPortal.SessionService();
            var session = service.Create(options);

            return Ok(session);
        }
    }
}