using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Model;
using StocksProcessing.API.Auth;
using StocksProcessing.API.Auth.Dtos;
using StocksProcessing.API.Email.Interfaces;
using StocksProcessing.API.Payloads;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StocksProcessing.API.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    public class AccountController : BaseController
    {

        private const string EmailNotConfirmedErrorMessage = "Email is not confirmed yet!";
        private const string FrontendResetPasswordUrl = "localhost:4200/auth/resetPassword";

        protected StocksMarketContext _mContext;
        protected UserManager<ApplicationUser> _userManager;
        protected SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        protected readonly IGeneralPurposeEmailService _emailSender;

        public AccountController(StocksMarketContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IGeneralPurposeEmailService emailSender)
        {
            _mContext = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        [HttpPost("register")]
        public async Task<ApiResponse<RegisterUserDataModelResponse>> RegisterAsync(
            [FromBody] RegisterUserDataModelRequest registerData)
        {
            string errorMessage = "Please provide every detail required for registration!";

            var errorResponse = new ApiResponse<RegisterUserDataModelResponse>()
            {
                ErrorMessage = errorMessage
            };

            if (registerData == null)
            {
                return errorResponse;
            }

            if (string.IsNullOrWhiteSpace(registerData.UserName))
            {
                return errorResponse;
            }

            var user = new ApplicationUser
            {
                UserName = registerData.UserName,
                FirstName = registerData.FirstName,
                LastName = registerData.LastName,
                Email = registerData.Email
            };

            var result = await _userManager.CreateAsync(user, registerData.Password);

            if (result.Succeeded)
            {
                var userIdentity = await _userManager.FindByNameAsync(user.UserName);

                //Generare token de verificarfe email...
                var emailConfirmationToken = await _userManager
                    .GenerateEmailConfirmationTokenAsync(userIdentity);

                //... si link confirmare
                var confirmUrl = Url.Action("ConfirmEmail", "Account",
                   new
                   {
                       email = user.Email,
                       token = emailConfirmationToken
                   }, protocol: HttpContext.Request.Scheme);

                //Trimite email catre user pentru confirmare
                var emailSendingResult = await _emailSender
                    .SendConfirmationEmail(userIdentity, confirmUrl);

                //Daca nu s-a trimis email-ul cu success
                if (!emailSendingResult.Successful)
                {
                    Debug.WriteLine($"Email couldn't be sent for user {userIdentity.UserName}");
                }

                return new ApiResponse<RegisterUserDataModelResponse>
                {
                    Response = new RegisterUserDataModelResponse
                    {
                        UserName = userIdentity.UserName,
                        FirstName = userIdentity.FirstName,
                        LastName = userIdentity.LastName,
                        Email = userIdentity.Email,
                        Token = "Bearer"
                    }
                };
            }
            else
            {
                return new ApiResponse<RegisterUserDataModelResponse>
                {
                    ErrorMessage = result.Errors.AggregateErrors()
                };
            }
        }

        [HttpPost("login")]
        public async Task<ApiResponse<UserProfileDetailsApiModel>> LoginAsync(
            [FromBody] LoginUserDataModel loginData)
        {
            string errorMessage = "Invalid username or password";

            var errorResponse = new ApiResponse<UserProfileDetailsApiModel>
            {
                ErrorMessage = errorMessage
            };

            if (loginData == null)
            {
                return errorResponse;
            }

            if (loginData?.UserNameOrEmail == null ||
                string.IsNullOrWhiteSpace(loginData.UserNameOrEmail))
            {
                return errorResponse;
            }

            //Este email?
            var isEmail = loginData.UserNameOrEmail.Contains("@");

            //Cauta userul dupa username sau email
            var user = isEmail ? await _userManager.FindByEmailAsync(loginData.UserNameOrEmail)
                : await _userManager.FindByNameAsync(loginData.UserNameOrEmail);


            if (user == null)
            {
                return errorResponse;
            }

            //Verifica daca parola este corecta fara a incrementa numarul de incercari. (peek)
            var isValidPassword = await _userManager.CheckPasswordAsync(user, loginData.Password);

            if (!isValidPassword)
                return errorResponse;

            //Verifica daca userul si-a confirmat adresa de email
            var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            //Daca nu si-a confirmat adresa de email...
            if (!isEmailConfirmed)
            {
                //Returneaza mesaj de eroare
                errorResponse.ErrorMessage = EmailNotConfirmedErrorMessage;

                return errorResponse;
            }


            //Returneaza token-ul
            return new ApiResponse<UserProfileDetailsApiModel>
            {
                Response = new UserProfileDetailsApiModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    UserName = user.UserName,
                    Token = user.GenerateJwtToken(_configuration["Jwt:Secret"],
                    _configuration["Jwt:Issuer"], _configuration["Jwt:Audience"])
                }
            };
        }

        [HttpGet("ConfirmEmail")]
        public async Task<ApiResponse> ConfirmEmail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var failedResponse = new ApiResponse
            {
                Response = "Confirmation Failed",
                ErrorMessage = "(Email, Token) pair didn't resolve to a valid instance.",
            };

            if (user == null)
            {
                return failedResponse;
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            return result.Succeeded ? new ApiResponse()
            {
                Response = "Confirmation Success"
            }
                                    : failedResponse;
        }



        [HttpPost("ForgotPasswordRequest")]
        public async Task<ApiResponse> ForgotPasswordRequest([FromBody] ModifyPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            var failedResponse = new ApiResponse
            {
                ErrorMessage = "Password reset failed.",
            };

            if (user is null)
            {
                return failedResponse;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var queryParams = new Dictionary<string, string>
            {
              {"token", token },
              {"email", request.Email }
            };

            var resetLink = QueryHelpers.AddQueryString(FrontendResetPasswordUrl, queryParams);

            var emailSendingResult = await _emailSender
                     .SendResetPasswordEmail(user, resetLink);

            if (!emailSendingResult.Successful)
            {
                return failedResponse;
            }

            return new ApiResponse()
            {
                Response = "Reset request taken. Email containing reset link sent."
            };
        }

        [HttpPost("ModifyPasswordRequest")]
        public async Task<ApiResponse> ModifyPasswordRequest([FromBody] ModifyPasswordRequest request)
        {
            return await ForgotPasswordRequest(request);
        }


        [HttpPost("ResetPassword")]
        public async Task<ApiResponse> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            var failedResponse = new ApiResponse
            {
                ErrorMessage = "Password reset failed.",
            };

            if (user is null)
            {
                return failedResponse;
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

            if (!result.Succeeded)
            {
                return new ApiResponse()
                {
                    ErrorMessage = result.Errors.AggregateErrors()
                };
            }

            return new ApiResponse()
            {
                Response = "Password changed."
            };
        }
    }
}
