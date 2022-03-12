using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Stocks.General.Models.Authentication;
using StocksFinalSolution.BusinessLogic.Interfaces.Email;
using StocksProccesing.Relational.Model;
using StocksProcessing.API.Auth;
using StocksProcessing.General.Exceptions;

namespace StocksFinalSolution.BusinessLogic.Features.Authentication;

public class UserPasswordResetService : IUserPasswordResetService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IGeneralPurposeEmailService _emailSender;

    public UserPasswordResetService(UserManager<ApplicationUser> userManager,
        IGeneralPurposeEmailService emailSender)
    {
        _userManager = userManager;
        _emailSender = emailSender;
    }

    public async Task ForgotPasswordRequest(ModifyPasswordRequest request,
        string frontEndUrlResetPasswordUrl)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            throw new UserNotFoundException("Invalid user email!");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var queryParams = new Dictionary<string, string>
        {
            {"token", token},
            {"email", request.Email}
        };

        var resetLink = QueryHelpers.AddQueryString(frontEndUrlResetPasswordUrl, queryParams);

        await _emailSender.SendResetPasswordEmail(user, resetLink);
    }

    public async Task ResetPassword(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            throw new UserNotFoundException("Invalid user email!");
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

        if (!result.Succeeded)
        {
            throw new InvalidPasswordResetLink(result.Errors.AggregateErrors());
        }
    }
}