using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stocks.General.ExtensionMethods;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;
using StocksProccesing.Relational.Model;
using StocksProcessing.API.Attributes;
using System;
using System.Threading.Tasks;

namespace StocksProcessing.API.Controllers.v1;


[ApiVersion("1.0")]
public class SuggestedTradeController: BaseController
{
    private readonly ITradeSuggestionsService _tradeSuggestionsService;
    private readonly UserManager<ApplicationUser> _userManager;

    public SuggestedTradeController(ITradeSuggestionsService tradeSuggestionsService,
        UserManager<ApplicationUser> userManager)
    {
        _tradeSuggestionsService = tradeSuggestionsService;
        _userManager = userManager;
    }

    [HttpGet("suggestions")]
    [AuthorizeToken]
    public async Task<IActionResult> GetTradeSuggestions (string ticker, string algorithm = "TS_SSA", string interval = "1h")
    {
        var userRequesting = await _userManager.GetUserAsync(HttpContext.User);
        TimeSpan intervalTimeSpan = TimeSpan.FromTicks(TimespanParser.ParseTimeSpanTicks(interval));

        return Ok(await _tradeSuggestionsService.DetermineViableTrades(userRequesting, ticker: ticker, 
            algorithm: algorithm, interval: intervalTimeSpan));
    }
}