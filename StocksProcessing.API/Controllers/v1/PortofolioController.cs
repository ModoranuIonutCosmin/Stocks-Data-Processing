using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stocks.General.Models.Funds;
using Stocks.General.Models.StocksInfoAggregates;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;
using StocksProccesing.Relational.Model;
using StocksProcessing.API.Attributes;

// For more information on enabling Web API for empty
// projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StocksProcessing.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[AuthorizeToken]
public class PortofolioController : BaseController
{
    private readonly IPortofolioService _portofolioService;
    private readonly UserManager<ApplicationUser> _userManager;

    public PortofolioController(IPortofolioService portofolioService,
        UserManager<ApplicationUser> userManager)
    {
        _portofolioService = portofolioService;
        _userManager = userManager;
    }

    [HttpPost("refillBalance")]
    public async Task<BalanceRefillOrder> ReplenishBalance([FromBody] PaymentDetails paymentDetails)
    {
        var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

        return await _portofolioService.ReplenishBalance(userRequesting, paymentDetails);
    }

    [HttpGet("previewPlaceOrder")]
    public async Task<OrderTaxesPreview> GetTransactionTaxes(string ticker, decimal invested,
        decimal leverage, bool isBuy)
    {
        var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

        return await _portofolioService.GetTransactionTaxes(userRequesting, ticker,
            invested, leverage, isBuy);
    }


    [HttpPost("placeOrder")]
    public async Task<PlaceMarketOrderRequest> PlaceOrder([FromBody] PlaceMarketOrderRequest marketOrder)
    {
        var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

        return await _portofolioService.PlaceOrder(userRequesting, marketOrder);
    }

    [HttpGet("tradingContext")]
    public async Task<TradingContext> GetTradingContext()
    {
        var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

        return _portofolioService.GetTradingContext(userRequesting);
    }
}