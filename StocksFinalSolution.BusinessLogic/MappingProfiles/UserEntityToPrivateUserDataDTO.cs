using AutoMapper;
using Stocks.General.Models.MyProfile;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.MappingProfiles;

public class UserEntityToPrivateUserDataDTO : Profile
{
    public UserEntityToPrivateUserDataDTO()
    {
        CreateMap<ApplicationUser, ProfilePrivateData>();
    }
}