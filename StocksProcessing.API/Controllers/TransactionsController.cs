using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stocks.General.Models;
using StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator;
using StocksProccesing.Relational.Model;
using StocksProccesing.Relational.Repositories;
using StocksProcessing.API.Auth;
using StocksProcessing.API.Auth.ExtensionMethods;
using StocksProcessing.API.Payloads;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StocksProcessing.API.Controllers
{
    [Route("api/v1/[controller]")]
    [AuthorizeToken]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IUsersRepository _userRepository;
        private readonly ITransactionSummaryCalculator _transactionSummaryCalculator;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly SignInManager<ApplicationUser> _signInManager;

        public readonly IStockMarketProfitCalculator _stockMarketProfitCalculator;

        public TransactionsController(IUsersRepository userRepository,
            ITransactionSummaryCalculator transactionSummaryCalculator,
            IStockMarketProfitCalculator stockMarketProfitCalculator,
            UserManager<ApplicationUser> userManager
           )
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _transactionSummaryCalculator = transactionSummaryCalculator;
            _stockMarketProfitCalculator = stockMarketProfitCalculator;
        }

        [HttpGet("openTransactions")]
        public async Task<ApiResponse<AllTransactionsGroupedSummary>> GatherTransactionsSummary()
        {
            var response = new ApiResponse<AllTransactionsGroupedSummary>();

            var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

            if (userRequesting is null)
            {
                response.ErrorMessage = "User is unauthorized!";

                Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                return response;
            }

            List<CompanyTransactionsSummary> transactionsSummary;

            try
            {
                List<StocksTransaction> openTransactionsList = _userRepository
                    .GetTransactionsListForUser(userRequesting);


                transactionsSummary = _transactionSummaryCalculator
                    .AggregateOpenTransactionsDataByCompaniesInfo(openTransactionsList);
            }
            catch (Exception e)
            {
                response.ErrorMessage = $"Couldn't get opened transactions data! | {e.Message}";

                return response;
            }

            response.Response = new AllTransactionsGroupedSummary
            {
                Transactions = transactionsSummary
            };

            return response;
        }

        [HttpGet("openTransactionsForTicker/{ticker}")]
        public async Task<ApiResponse<AllTransactionsDetailed>> GatherTransactionsParticularTicker(string ticker)
        {
            var response = new ApiResponse<AllTransactionsDetailed>();


            var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

            if (userRequesting is null)
                return this.FailedApiOperationResponse<AllTransactionsDetailed>("User is unauthorized!",
                    statusCode: HttpStatusCode.Unauthorized);


            var result = default(AllTransactionsDetailed);

            try
            {
                var openTransactionsList = new List<StocksTransaction>();

                openTransactionsList = _userRepository
                    .GetTransactionsListForUserByTicker(userRequesting, ticker);

                result = _transactionSummaryCalculator
                    .AggregateOpenTransactionsDataForSingleCompany(openTransactionsList, ticker);

            }
            catch (Exception e)
            {
                response.ErrorMessage = $"Couldn't get opened transactions data! | {e.Message}";

                return response;
            }

            response.Response = result;

            return response;
        }

        [HttpPost("closeTransaction")]
        public async Task<ApiResponse<CloseTransactionRequest>> CancelTransaction([FromBody] CloseTransactionRequest request)
        {
            var response = new ApiResponse<CloseTransactionRequest>();

            try
            {
                var transaction = _userRepository.GetTransactionInfo(request.Id);

                if (transaction is null)
                    throw new NullReferenceException(nameof(transaction));

                var userRequesting = await _userManager.GetUserAsync(HttpContext.User);

                if (userRequesting.UserOwnsTheTransaction(transaction))
                {

                    response.ErrorMessage = "User is unauthorized!";

                    Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                    return response;
                }

                decimal profitOrLoss = _stockMarketProfitCalculator.CalculateTransactionProfit(transaction);

                await _userRepository.CloseUserTransaction(userRequesting, transaction, profitOrLoss);
            }

            catch (Exception ex)
            {
                response.ErrorMessage = $"An error occured. Please try again later. | {ex.Message}";

                return response;
            }

            response.Response = request;

            return response;
        }
    }
}
