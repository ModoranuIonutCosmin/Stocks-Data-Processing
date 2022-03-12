using System.Threading.Tasks;
using Stocks.General.Models.MyProfile;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Services;

public interface IProfileService
{
    Task<ProfilePrivateData> GatherProfileData(ApplicationUser requestingUser);
}