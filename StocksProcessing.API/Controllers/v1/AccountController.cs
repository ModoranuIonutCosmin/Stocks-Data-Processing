using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StocksProcessing.API.Auth.Dtos;
using StocksProcessing.API.Payloads;
using System.Threading.Tasks;
using StocksFinalSolution.BusinessLogic.Features.Authentication;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StocksProcessing.API.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    public class AccountController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IUserAuthenticationService _userAuthenticationService;
        private readonly IUserPasswordResetService _passwordResetService;

        public AccountController(
            IConfiguration configuration,
            IUserAuthenticationService userAuthenticationService,
            IUserPasswordResetService passwordResetService
        )
        {
            _configuration = configuration;
            _userAuthenticationService = userAuthenticationService;
            _passwordResetService = passwordResetService;
        }

        [HttpPost("register")]
        public async Task<RegisterUserDataModelResponse> RegisterAsync(
            [FromBody] RegisterUserDataModelRequest registerData)
        {
            string frontEndUrl = (Environment.GetEnvironmentVariable("FrontEndUrl") ??
                                  _configuration["FrontEnd:URL"]) +
                                 _configuration["FrontEnd:ConfirmationRoute"];

            return await _userAuthenticationService.RegisterAsync(registerData, frontEndUrl);
        }

        [HttpPost("login")]
        public async Task<UserProfileDetailsApiModel> LoginAsync(
            [FromBody] LoginUserDataModel loginData)
        {
            string issuer = Environment.GetEnvironmentVariable("JwtIssuer") ?? _configuration["Jwt:Issuer"];
            string audience = Environment.GetEnvironmentVariable("JwtAudience") ?? _configuration["Jwt:Audience"];
            string secret = Environment.GetEnvironmentVariable("JwtSecret") ?? _configuration["Jwt:Secret"];

            return 
                await _userAuthenticationService.LoginAsync(loginData, secret, issuer, audience);
        }

        [HttpPost("ConfirmEmail")]
        public async Task<ApiResponse> ConfirmEmail([FromBody] ConfirmEmailRequest confirmEmailRequest)
        {
            await _userAuthenticationService.ConfirmEmail(confirmEmailRequest.Email,
                confirmEmailRequest.Token);

            return new ApiResponse()
            {
                Response = "Email confirmed!"
            };
        }

        [HttpPost("ForgotPassword")]
        public async Task<ApiResponse> ForgotPasswordRequest([FromBody] ModifyPasswordRequest request)
        {
            string frontEndUrl = (Environment.GetEnvironmentVariable("FrontEndUrl") ??
                                  _configuration["FrontEnd:URL"]) +
                                 _configuration["FrontEnd:ResetPasswordRoute"];

            await _passwordResetService.ForgotPasswordRequest(request,
                frontEndUrl);

            return new ApiResponse()
            {
                Response = "Password reset email sent!"
            };
        }

        [HttpPut("ResetPassword")]
        public async Task<ApiResponse> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            await _passwordResetService.ResetPassword(request);

            return new ApiResponse()
            {
                Response = "Password changed successfully."
            };
        }
    }
}