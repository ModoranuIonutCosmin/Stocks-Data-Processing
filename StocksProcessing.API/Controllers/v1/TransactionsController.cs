using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stocks.General.Models.Transactions;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;
using StocksProccesing.Relational.Model;
using StocksProcessing.API.Auth;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StocksProcessing.API.Controllers.v1;

[AuthorizeToken]
[ApiController]
[ApiVersion("1.0")]
public class TransactionsController : BaseController
{
    private readonly ITransactionsService _transactionsService;
    protected readonly UserManager<ApplicationUser> _userManager;


    public TransactionsController(ITransactionsService transactionsService,
        UserManager<ApplicationUser> userManager
    )
    {
        _transactionsService = transactionsService;
        _userManager = userManager;
    }

    [HttpGet("openTransactions")]
    public async Task<AllTransactionsGroupedSummary> GatherTransactionsSummary()
    {
        var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

        return await _transactionsService.GatherTransactionsSummary(userRequesting);
    }

    [HttpGet("openTransactionsForTicker/{ticker}")]
    public async Task<AllTransactionsDetailed> GatherTransactionsParticularTicker(string ticker)
    {
        var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

        return await _transactionsService.GatherTransactionsParticularTicker(userRequesting, ticker);
    }

    [HttpPost("closeTransaction")]
    public async Task<CloseTransactionRequest> CancelTransaction(
        [FromBody] CloseTransactionRequest request)
    {
        var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

        return await _transactionsService.CancelTransaction(userRequesting, request);
    }
}