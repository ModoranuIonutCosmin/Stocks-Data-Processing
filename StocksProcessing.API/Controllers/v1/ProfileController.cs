using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stocks.General.Models.MyProfile;
using StocksFinalSolution.BusinessLogic.Features.MyProfile;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;
using StocksProccesing.Relational.Model;
using StocksProcessing.API.Auth;

namespace StocksProcessing.API.Controllers.v1;

[ApiVersion("1.0")]
[AuthorizeToken]
public class ProfileController : BaseController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IProfileService _profileService;

    public ProfileController(UserManager<ApplicationUser> userManager,
        IProfileService profileService)
    {
        _userManager = userManager;
        _profileService = profileService;
    }
    
    [HttpGet("info")]
    public async Task<ProfilePrivateData> GatherProfileData()
    {
        ApplicationUser requestingUser = await _userManager.GetUserAsync(HttpContext.User);

        return await _profileService.GatherProfileData(requestingUser);
    }

}