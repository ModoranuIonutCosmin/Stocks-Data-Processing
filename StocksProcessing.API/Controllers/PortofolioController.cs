using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Model;
using StocksProcessing.API.Auth;
using System.Threading.Tasks;

// For more information on enabling Web API for empty
// projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StocksProcessing.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PortofolioController : ControllerBase
    {
        private readonly StocksMarketContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public PortofolioController(StocksMarketContext context,
            UserManager<ApplicationUser> userManager)
        {
            _dbContext = context;
            _userManager = userManager;
        }

        [AuthorizeToken]
        [HttpGet]
        public async Task<IActionResult> GetAllMinutelyData(string companyTicker)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            return Content($"Salot {user.FirstName}!");
        }
    }
}
